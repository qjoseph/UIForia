﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UIForia.Util.Unsafe;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UIForia.Util {

    public struct LineInfo {

        public readonly int line;
        public readonly int column;

        public LineInfo(int line, int column) {
            this.line = line;
            this.column = column;
        }

        public override string ToString() {
            return line + ":" + column;
        }

    }

    public enum CommentMode {

        None,
        DoubleSlash,
        XML

    }

    public unsafe struct CharStream {

        private char* data;
        private uint dataStart;
        private uint dataEnd;
        private uint ptr;
        public int baseOffset;
        public CommentMode commentMode;

        [ThreadStatic] private static string s_ScratchBuffer;
        [ThreadStatic] private static List<EnumNameEntry> s_EnumNameEntryList;

        public CharStream(CharSpan span) : this() {
            this.data = span.data;
            this.dataStart = span.rangeStart;
            this.dataEnd = span.rangeEnd;
            this.ptr = dataStart;
            this.commentMode = CommentMode.DoubleSlash;
        }

        public CharStream(char* source, int start, int end) : this() {
            this.data = source;
            this.dataStart = (uint) start;
            this.dataEnd = (uint) end;
            this.ptr = dataStart;
            this.commentMode = CommentMode.DoubleSlash;
        }

        public CharStream(char* source, uint start, uint end) : this() {
            this.data = source;
            this.dataStart = start;
            this.dataEnd = end;
            this.ptr = dataStart;
            this.commentMode = CommentMode.DoubleSlash;
        }

        // copy from current pointer position
        public CharStream(CharStream propertyStream) : this() {
            this.data = propertyStream.data;
            this.dataStart = propertyStream.ptr;
            this.dataEnd = propertyStream.dataEnd;
            this.ptr = propertyStream.ptr;
            this.commentMode = CommentMode.DoubleSlash;
        }

        public bool HasMoreTokens => ptr < dataEnd;
        public uint Size => dataEnd - dataStart;
        public char* Data => data;
        public uint Ptr => ptr;
        public uint End => dataEnd;
        public int IntPtr => (int) ptr;
        public char Last => data[dataEnd - 1];

        public char Current {
            get => data[ptr];
        }

        public char Next {
            get => ptr + 1 < dataEnd ? data[ptr + 1] : '\0';
        }

        public char Previous {
            get {
                int i = (int) ptr - 1;
                int ds = (int) dataStart;
                return i >= ds ? data[i] : '\0';
            }
        }

        public char this[uint idx] {
            get => data[idx];
        }

        public void Advance(uint advance = 1) {
            ptr += advance;
            if (ptr >= dataEnd) {
                ptr = dataEnd;
            }
        }

        public void AdvanceTo(uint target) {
            if (target < ptr) return;
            ptr = target;
            if (ptr >= dataEnd) {
                ptr = dataEnd;
            }
        }

        public void AdvanceTo(int target) {
            if (target < ptr) return;
            ptr = (uint) target;
            if (ptr >= dataEnd) {
                ptr = dataEnd;
            }
        }

        public void AdvanceSkipWhitespace(uint advance = 1) {
            ptr += advance;
            if (ptr >= dataEnd) {
                ptr = dataEnd;
            }

            while (ptr < dataEnd && char.IsWhiteSpace(data[ptr])) {
                ptr++;
            }
        }

        public bool TryMatchRange(string str) {
            if (ptr + str.Length > dataEnd) {
                return false;
            }

            fixed (char* s = str) {
                if (UnsafeUtility.MemCmp(s, data + ptr, str.Length * 2) != 0) {
                    return false;
                }

                Advance((uint) str.Length);
                return true;
            }
        }

        public bool TryMatchRange(in CharSpan charSpan) {
            if (ptr + charSpan.Length > dataEnd) {
                return false;
            }

            for (int i = 0; i < charSpan.Length; i++) {
                if (data[ptr + i] != charSpan[i]) {
                    return false;
                }
            }

            Advance((uint) charSpan.Length);
            return true;
        }

        public bool TryMatchRange(string str, out uint advance) {
            if (ptr + str.Length > dataEnd) {
                advance = 0;
                return false;
            }

            fixed (char* s = str) {
                for (int i = 0; i < str.Length; i++) {
                    if (data[ptr + i] != *s) {
                        advance = 0;
                        return false;
                    }
                }
            }

            advance = (uint) str.Length;
            return true;
        }

        public bool TryMatchRange(string str, char optional, out bool usedOptional, out uint advance) {
            if (TryMatchRange(str, out advance)) {
                if (data[ptr] == optional) {
                    usedOptional = true;
                    advance += 1;
                }
            }

            usedOptional = false;
            advance = 0;
            return false;
        }

        public static bool operator ==(CharStream stream, char character) {
            if (stream.ptr >= stream.dataEnd) return false;
            return stream.data[stream.ptr] == character;
        }

        public static bool operator !=(CharStream stream, char character) {
            return !(stream == character);
        }

        public bool TryGetSubStream(char open, char close, out CharStream charStream, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            uint start = ptr;
            if ((whitespaceHandling & WhitespaceHandling.ConsumeBefore) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            if (data[ptr] != open) {
                charStream = default;
                RewindTo(start);
                return false;
            }

            uint i = ptr;
            int counter = 1;
            while (i < dataEnd) {
                i++;

                if (data[i] == open) {
                    counter++;
                }
                else if (data[i] == close) {
                    counter--;
                    if (counter == 0) {
                        charStream = new CharStream(data, ptr + 1, i);
                        ptr = i + 1; // step over close
                        if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                            ConsumeWhiteSpaceAndComments();
                        }

                        return true;
                    }
                }
            }

            RewindTo(start);
            charStream = default;
            return false;
        }

        public void RewindTo(uint start) {
            ptr = start < dataStart ? dataStart : start;
        }

        public void ConsumeWhiteSpaceAndComments(CommentMode commentMode) {
            CommentMode lastMode = this.commentMode;
            this.commentMode = commentMode;
            ConsumeWhiteSpaceAndComments();
            this.commentMode = lastMode;
        }

        private static bool IsSpace(char c) {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        public void ConsumeWhiteSpaceAndComments() {
            while (true) {
                while (ptr < dataEnd && char.IsWhiteSpace(data[ptr])) {
                    ptr++;
                }

                if (commentMode == CommentMode.DoubleSlash && ptr < dataEnd - 2) {
                    if (data[ptr + 0] == '/' && data[ptr + 1] == '/') {
                        ptr += 2;
                        while (ptr < dataEnd) {
                            if (data[ptr] == '\n') {
                                break;
                            }

                            ptr++;
                        }

                        continue;
                    }
                }
                else if (commentMode == CommentMode.XML) {
                    ConsumeXMLComment();
                }

                break;
            }
        }

        public void ConsumeXMLComment() {
            if (data[ptr] == '<' && ptr + 4 < dataEnd && data[ptr + 1] == '!' && data[ptr + 2] == '-' && data[ptr + 3] == '-') {
                ptr += 4;

                while (ptr + 3 < dataEnd) {
                    if (data[ptr] == '-' && data[ptr + 1] == '-' && data[ptr + 2] == '>') {
                        ptr += 3;
                        return;
                    }

                    ptr++;
                }
            }

            // while (ptr < dataEnd) {
            //     char current = data[ptr];
            //     if (!(current == ' ' || current == '\t' || current == '\n' || current == '\r')) {
            //         if (data[ptr] == '<' && ptr + 4 < dataEnd && data[ptr + 1] == '!' && data[ptr + 2] == '-' && data[ptr + 3] == '-') {
            //             ptr += 4; // step over <!--
            //
            //             while (ptr + 2 < dataEnd && !(data[ptr] == '-' && data[ptr + 1] == '-')) {
            //                 ptr++;
            //             }
            //
            //             ptr += 2; // step over --
            //         }
            //         else {
            //             break;
            //         }
            //     }
            //
            //     ptr++;
            // }
        }

        public bool TryGetSubstreamTo(char c0, char c1, out CharStream stream) {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == c0 || c == c1) {
                    stream = new CharStream(data, ptr, i);
                    Advance(i - ptr + 1);
                    return true;
                }

                i++;
            }

            stream = default;
            return false;
        }

        public bool TryGetCharSpanTo(char c0, char c1, out CharSpan span) {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == c0 || c == c1) {
                    span = new CharSpan(data, (int) ptr, (int) i);
                    Advance(i - ptr + 1);
                    return true;
                }

                i++;
            }

            span = default;
            return false;
        }

        public bool TryGetCharSpanTo(char c0, out CharSpan span) {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == c0) {
                    span = new CharSpan(data, (int) ptr, (int) i);
                    Advance(i - ptr + 1);
                    return true;
                }

                i++;
            }

            span = default;
            return false;
        }

        public bool TryGetSubstreamTo(char c0, out CharStream stream) {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == c0) {
                    stream = new CharStream(data, ptr, i);
                    Advance(i - ptr + 1);
                    return true;
                }

                i++;
            }

            stream = default;
            return false;
        }

        public bool TryGetDelimitedSubstream(char target, out CharStream stream, char c1 = '\0', char c2 = '\0', char c3 = '\0') {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == target) {
                    stream = new CharStream(data, ptr, i);
                    Advance(i - ptr + 1);
                    return true;
                }

                if (c == c1 || c == c2 || c == c3) {
                    stream = default;
                    return false;
                }

                i++;
            }

            stream = default;
            return false;
        }

        public bool TryGetStreamUntil(out CharStream stream, out char end, char c1, char c2 = '\0', char c3 = '\0') {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == c1 || c == c2 || c == c3) {
                    end = c;
                    stream = new CharStream(data, ptr, i);
                    Advance(i - ptr + 1);
                    return true;
                }

                i++;
            }

            end = default;
            stream = default;
            return false;
        }

        public bool TryGetStreamUntil(char terminator, out CharStream span, char escape) {
            int i = (int) ptr;
            char prev = '\0';
            if (i - 1 >= dataStart) {
                prev = data[i - 1];
            }

            while (i < dataEnd) {
                char c = data[i];
                if (c == terminator && prev != escape) {
                    span = new CharStream(data, (ushort) ptr, (ushort) i);
                    ptr = (uint) i;
                    return true;
                }

                prev = c;
                i++;
            }

            span = default;
            return false;
        }

        public bool TryGetStreamUntil(char terminator, out CharStream span) {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == terminator) {
                    span = new CharStream(data, (ushort) ptr, (ushort) i);
                    ptr = i;
                    return true;
                }

                i++;
            }

            span = default;
            return false;
        }

        public bool TryGetStreamUntil(out CharSpan span, char c1, char c2 = '\0', char c3 = '\0') {
            uint i = ptr;
            while (i < dataEnd) {
                char c = data[i];
                if (c == c1 || c == c2 || c == c3 || char.IsWhiteSpace(c)) {
                    span = new CharSpan(data, (ushort) ptr, (ushort) i);
                    ptr = i;
                    return true;
                }

                i++;
            }

            span = default;
            return false;
        }

        public int NextIndexOf(char c) {
            uint i = ptr;
            while (i < dataEnd) {
                if (data[i] == c) {
                    return (int) i;
                }

                i++;
            }

            return -1;
        }

        public bool Contains(char c) {
            return NextIndexOf(c) != -1;
        }

        public int GetLineNumber() {
            int line = 0;
            for (int i = 0; i < ptr; i++) {
                if (data[i] == '\n') line++;
            }

            return line;
        }

        public int GetStartLineNumber() {
            int line = 0;
            for (int i = 0; i < dataStart; i++) {
                if (data[i] == '\n') line++;
            }

            return line;
        }

        public int GetEndLineNumber() {
            int line = 0;
            for (int i = 0; i < dataEnd; i++) {
                if (data[i] == '\n') line++;
            }

            return line;
        }

        private const int s_ScratchBufferLength = 128;

        private struct EnumNameEntry {

            public Type type;
            public string[] names;
            public int[] values;

        }

        public bool TryParseMultiDottedIdentifier(out CharSpan retn, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll, bool allowMinus = false) {
            uint start = ptr;

            if ((whitespaceHandling & WhitespaceHandling.ConsumeBefore) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            if (!TryParseIdentifier(out CharSpan identifier, allowMinus, WhitespaceHandling.ConsumeBefore)) {
                retn = default;
                return false;
            }

            int end = identifier.rangeEnd;

            while (TryParseCharacter('.', WhitespaceHandling.None)) {
                if (!TryParseIdentifier(out CharSpan endIdent, allowMinus, WhitespaceHandling.None)) {
                    retn = default;
                    ptr = start;
                    return false;
                }

                end = endIdent.rangeEnd;
            }

            retn = new CharSpan(identifier.data, identifier.rangeStart, end);
            if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            return true;
        }

        public bool TryParseDottedIdentifier(out CharSpan retn, out bool wasDotted, bool allowMinus = true) {
            if (!TryParseIdentifier(out CharSpan identifier, allowMinus, WhitespaceHandling.ConsumeBefore)) {
                retn = default;
                wasDotted = false;
                return false;
            }

            if (!TryParseCharacter('.', WhitespaceHandling.None)) {
                retn = identifier;
                wasDotted = false;
                return true;
            }

            if (!TryParseIdentifier(out CharSpan endIdent, allowMinus, WhitespaceHandling.ConsumeAfter)) {
                ConsumeWhiteSpaceAndComments();
                wasDotted = false;
                retn = identifier;
                return true;
            }

            wasDotted = true;
            retn = new CharSpan(data, identifier.rangeStart, endIdent.rangeEnd);
            return true;
        }

        public bool TryParseDottedIdentifier(out CharSpan retn) {
            return TryParseDottedIdentifier(out retn, out bool _);
        }

        public bool TryParseIdentifier(out CharSpan span, bool allowMinus = true, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            uint start = ptr;

            if ((whitespaceHandling & WhitespaceHandling.ConsumeBefore) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            if (TryParseIdentifier(out int rangeStart, out int rangeEnd, allowMinus)) {
                span = new CharSpan(data, rangeStart, rangeEnd);
                if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                    ConsumeWhiteSpaceAndComments();
                }

                return true;
            }

            RewindTo(start);
            span = default;
            return false;
        }

        public bool TryParseIdentifier(out int rangeStart, out int rangeEnd, bool allowMinus = true) {
            char first = data[ptr];

            if (!char.IsLetter(first) && first != '_') {
                rangeStart = -1;
                rangeEnd = -1;
                return false;
            }

            uint ptr2 = ptr;
            while (ptr2 < End) {
                char c = data[ptr2];
                if (!char.IsLetterOrDigit(c)) {
                    if (c == '_' || (allowMinus && c == '-')) {
                        ptr2++;
                        continue;
                    }

                    break;
                }

                ptr2++;
            }

            uint length = ptr2 - ptr;
            if (length > 0) {
                rangeStart = (int) ptr;
                rangeEnd = (int) ptr2;
                Advance(length);
                return true;
            }

            rangeStart = -1;
            rangeEnd = -1;
            return false;
        }

        public bool TryParseByte(out byte value) {
            if (TryParseUInt(out uint val) && val <= byte.MaxValue) {
                value = (byte) val;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryParseUShort(out ushort value) {
            if (TryParseUInt(out uint val) && val <= ushort.MaxValue) {
                value = (ushort) val;
                return true;
            }

            value = 0;
            return false;
        }

        public bool TryParseFloat(out float value) {
            if (s_ScratchBuffer == null) {
                s_ScratchBuffer = new string('\0', s_ScratchBufferLength);
            }

            uint start = ptr;

            // oh, you thought C# strings were immutable? :)

            // writing a float.Parse function is hard and error prone so I want to the use the C# built in one.
            // Somebody at Microsoft thought it would be a good idea to only support parsing float from strings though
            // and didn't consider the character range use case that we need. So I take a string, set its contents
            // to my data, pass that string to float.TryParse, and use the result. 
            int cnt = 0;
            fixed (char* charptr = s_ScratchBuffer) {
                uint idx = start;
                int dotIndex = -1;
                if (data[ptr] == '-') {
                    charptr[cnt++] = '-';
                    idx++;
                }

                // read until end or whitespace
                while (idx < dataEnd && cnt < s_ScratchBufferLength) {
                    char c = data[idx];
                    if (c < '0' || c > '9') {
                        if (c == '.' && dotIndex == -1) {
                            dotIndex = (int) idx;
                            charptr[cnt++] = c;
                            idx++;
                            continue;
                        }

                        break;
                    }

                    charptr[cnt++] = c;
                    idx++;
                }

                bool retn = float.TryParse(s_ScratchBuffer, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);

                // reset the scratch buffer so the next call has valid state.
                // treat our charptr as long * so that we can clear it in fewer operations (char = 16 bits, long = 64)
                // avoiding divide, also avoiding Math.Ceil call
                // instead just clamp to bufferSize / sizeof(long) which happens to be 16
                int longCnt = (int) (cnt * 0.25f) + 1;
                long* longptr = (long*) charptr;

                if (longCnt > 16) longCnt = 16;

                for (int i = 0; i < longCnt; i++) {
                    longptr[i] = 0;
                }

                if (retn) {
                    Advance((uint) cnt);
                    TryParseCharacter('f');
                }

                return retn;
            }
        }

        public bool TryParseUInt(out uint intVal) {
            uint i = ptr;

            // read until end or whitespace
            while (i < dataEnd) {
                char c = data[i];
                if (c < '0' || c > '9') {
                    break;
                }

                i++;
            }

            if (i == ptr) {
                intVal = default;
                return false;
            }

            uint length = i - ptr;
            Advance(length);

            int number = 0;
            int multiplier = 1;

            while (length-- != 0) {
                number += (data[length] - '0') * multiplier;
                multiplier *= 10;
            }

            intVal = (uint) number;

            return true;
        }

        public bool TryParseInt(out int intVal) {
            int sign = 1;
            uint minusSize = 0;
            uint i = ptr;

            if (data[ptr] == '-') {
                sign = -1;
                minusSize = 1;
                i++;
            }

            // read until end or whitespace
            while (i < dataEnd) {
                char c = data[i];
                if (c < '0' || c > '9') {
                    break;
                }

                i++;
            }

            if (i == ptr) {
                intVal = default;
                return false;
            }

            uint length = i - ptr;
            Advance(length);

            int number = 0;
            int mult = 1;

            while (length-- != minusSize) {
                number += (data[length] - '0') * mult;
                mult *= 10;
            }

            intVal = number * sign;

            return true;
        }

        // Cannot cast T to int to T without boxing, so we return the integer and expect the caller to cast to enum type
        public bool TryParseEnum<T>(out int enumValue) where T : Enum {
            if (s_EnumNameEntryList == null) {
                s_EnumNameEntryList = new List<EnumNameEntry>();
            }

            string[] names = null;
            int[] values = null;

            for (int i = 0; i < s_EnumNameEntryList.Count; i++) {
                EnumNameEntry nameEntry = s_EnumNameEntryList[i];
                if (typeof(T) == nameEntry.type) {
                    names = nameEntry.names;
                    values = nameEntry.values;
                    break;
                }
            }

            if (names == null) {
                names = Enum.GetNames(typeof(T));
                values = (int[]) Enum.GetValues(typeof(T));
                s_EnumNameEntryList.Add(new EnumNameEntry() {
                    type = typeof(T),
                    names = names,
                    values = values
                });
            }

            if (!TryParseIdentifier(out int rangeStart, out int rangeEnd)) {
                enumValue = default;
                return false;
            }

            for (int i = 0; i < names.Length; i++) {
                string name = names[i];
                if (StringUtil.EqualsRangeUnsafe(name, data, rangeStart, rangeEnd - rangeStart)) {
                    enumValue = values[i];
                    return true;
                }
            }

            enumValue = default;
            return false;
        }

        public bool TryParseCharacter(char character, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            uint save = ptr;

            if ((whitespaceHandling & WhitespaceHandling.ConsumeBefore) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            if (data[ptr] == character) {
                Advance();

                if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                    ConsumeWhiteSpaceAndComments();
                }

                return true;
            }

            RewindTo(save);

            return false;
        }

        public bool TryParseColorProperty(out Color32 color) {
            uint start = ptr;

            ConsumeWhiteSpaceAndComments();

            if (TryMatchRange("rgb", 'a', out bool usedOptional, out uint advance)) {
                AdvanceSkipWhitespace(advance);

                if (TryGetSubStream('(', ')', out CharStream signature)) {
                    Advance(signature.Size + 1);

                    byte a = 255;

                    // expect four comma separated floats or integers

                    if (!signature.TryParseByte(out byte r)) {
                        goto fail;
                    }

                    if (!signature.TryParseCharacter(',')) {
                        goto fail;
                    }

                    if (!signature.TryParseByte(out byte g)) {
                        goto fail;
                    }

                    if (!signature.TryParseCharacter(',')) {
                        goto fail;
                    }

                    if (!signature.TryParseByte(out byte b)) {
                        goto fail;
                    }

                    if (usedOptional && (!signature.TryParseCharacter(',') || !signature.TryParseByte(out a))) {
                        goto fail;
                    }

                    color = new Color32(r, g, b, a);
                }
            }
            else if (data[ptr] == '#') {
                // read 6 or 8 characters
                // https://www.includehelp.com/code-snippets/convert-hexadecimal-string-to-integer-in-c-programming.aspx

                char* buffer = stackalloc char[8];

                int i = 0;
                bool valid = true;
                for (i = 0; i < 8; i++) {
                    if (ptr + i <= dataEnd) {
                        break;
                    }

                    char c = data[ptr + i];

                    if (c >= '0' && c <= '9') {
                        buffer[i] = c;
                    }
                    else {
                        switch (c) {
                            case 'A':
                            case 'a':
                            case 'B':
                            case 'b':
                            case 'C':
                            case 'c':
                            case 'D':
                            case 'd':
                            case 'E':
                            case 'e':
                            case 'F':
                            case 'f':
                                buffer[i] = c;
                                break;

                            default: {
                                valid = false;
                                break;
                            }
                        }
                    }
                }

                if (!valid || (i != 6 && i != 8)) {
                    color = default;
                    return false;
                }

                if (i == 6) {
                    buffer[6] = 'F';
                    buffer[7] = 'F';
                }

                int digit = 0;
                int intValue = 0;

                for (int x = 7, p = 0; x >= 0; x--, p++) {
                    char c = buffer[x];
                    if (c >= '0' || c <= '9') {
                        digit = c - 0x30;
                    }
                    else {
                        switch (c) {
                            case 'A':
                            case 'a':
                                digit = 10;
                                break;

                            case 'B':
                            case 'b':
                                digit = 11;
                                break;

                            case 'C':
                            case 'c':
                                digit = 12;
                                break;

                            case 'D':
                            case 'd':
                                digit = 13;
                                break;

                            case 'E':
                            case 'e':
                                digit = 14;
                                break;

                            case 'F':
                            case 'f':
                                digit = 15;
                                break;
                        }
                    }

                    intValue = digit * (int) Mathf.Pow(16, p);
                }

                color = ColorUtil.ColorFromInt(intValue);
                return true;
            }

            else if (ColorUtil.TryParseColorName(new CharSpan(data, (int) ptr, (int) dataEnd), out color, out int nameLength)) {
                Advance((uint) nameLength);
                return true;
            }

            fail:
            RewindTo(start);
            color = default;
            return false;
        }

        public override string ToString() {
            return new string(data, (int) ptr, (int) (dataEnd - ptr));
        }

        public bool TryParseXMLClose(out CharSpan identifier) {
            if (ptr + 4 < dataEnd) {
                identifier = default;
                return false;
            }

            if (data[ptr] != '<' || data[ptr + 1] != '/') {
                identifier = default;
                return false;
            }

            while (ptr < dataEnd && char.IsWhiteSpace(data[ptr])) {
                ptr++;
            }

            if (!TryParseIdentifier(out identifier, true, WhitespaceHandling.ConsumeAfter)) {
                return false;
            }

            return TryParseCharacter('>');
        }

        public bool TryParseXMLAttribute(out CharSpan key, out CharSpan value, bool requireQuotes = false) {
            uint start = ptr;

            while (ptr < dataEnd && char.IsWhiteSpace(data[ptr])) {
                ptr++;
            }

            if (!TryParseIdentifier(out key)) {
                value = default;
                ptr = start;
                return false;
            }

            if (!TryParseCharacter('=')) {
                value = default;
                ptr = start;
                return false;
            }

            if (requireQuotes) {
                if (!TryParseCharacter('"')) {
                    value = default;
                    ptr = start;
                    return false;
                }

                if (TryGetCharSpanTo('"', out value)) {
                    return true;
                }
            }
            // read until we hit a space, > or end of input
            else if (TryGetCharSpanTo(' ', '>', out value)) {
                return true;
            }

            ptr = start;
            return false;
        }

        public bool TryParseXMLOpenTagIdentifier(out CharSpan identifier) {
            if (data[ptr] != '<') {
                identifier = default;
                return false;
            }

            ptr++;
            if (TryParseIdentifier(out identifier, true, WhitespaceHandling.ConsumeAfter)) {
                return true;
            }

            return false;
        }

        public bool TryMatchRangeIgnoreCase(string str, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            uint start = ptr;
            if ((whitespaceHandling & WhitespaceHandling.ConsumeBefore) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            if (str.Length == 1 && ptr <= dataEnd && str[0] == data[ptr]) {
                ptr++;
                if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                    ConsumeWhiteSpaceAndComments();
                }

                return true;
            }

            if (ptr + str.Length > dataEnd) {
                ptr = start;
                return false;
            }

            fixed (char* s = str) {
                for (int i = 0; i < str.Length; i++) {
                    char strChar = s[i];
                    char dataChar = data[ptr + i];

                    char c1 = strChar >= 'a' && strChar <= 'z' ? char.ToLower(strChar) : strChar;
                    char c2 = dataChar >= 'a' && dataChar <= 'z' ? char.ToLower(dataChar) : dataChar;

                    if (c1 != c2) {
                        ptr = start;
                        return false;
                    }
                }
            }

            Advance((uint) str.Length);

            if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            return true;
        }

        public void Rewind(uint i) {
            ptr -= i;
            if (ptr < dataStart) ptr = dataStart;
        }

        public void RemoveLast() {
            if (dataEnd <= dataStart) {
                return;
            }

            dataEnd--;
        }

        public LineInfo GetLineInfo() {
            int line = 1; // start counting at line 1
            int x = 0;

            for (int i = 0; i < ptr; i++) {
                if (data[i] == '\n') {
                    line++;
                    x = i + 1;
                }
            }

            int col = 1;
            for (int i = x; i < ptr - 1; i++) {
                col++;
            }

            return new LineInfo(line, col);
        }

        public bool TryParseFixedLength(out UIFixedLength fixedLength, bool allowUnitless) {
            uint start = ptr;
            if (TryParseFloat(out float value)) {
                bool gotUnit = TryParseFixedLengthUnit(out UIFixedUnit unit);
                if (gotUnit || allowUnitless) {
                    fixedLength = new UIFixedLength(value, unit);
                    return true;
                }
            }

            ptr = start;
            fixedLength = default;
            return false;
        }

        public bool TryParseFixedLengthUnit(out UIFixedUnit fixedUnit) {
            while (ptr < dataEnd && char.IsWhiteSpace(data[ptr])) {
                ptr++;
            }

            if (ptr >= dataEnd) {
                fixedUnit = default;
                return false;
            }

            if (data[ptr] == '%') {
                fixedUnit = UIFixedUnit.Percent;
                ptr++;
                return true;
            }

            if (ptr + 1 >= dataEnd) {
                fixedUnit = default;
                ptr += 2;
                return false;
            }

            if (data[ptr] == 'e' && data[ptr + 1] == 'm') {
                fixedUnit = UIFixedUnit.Em;
                ptr += 2;
                return true;
            }

            if (data[ptr] == 'p' && data[ptr + 1] == 'x') {
                fixedUnit = UIFixedUnit.Pixel;
                ptr += 2;
                return true;
            }

            if (data[ptr] == 'v' && data[ptr + 1] == 'w') {
                fixedUnit = UIFixedUnit.ViewportWidth;
                ptr += 2;
                return true;
            }

            if (data[ptr] == 'v' && data[ptr + 1] == 'h') {
                fixedUnit = UIFixedUnit.ViewportHeight;
                ptr += 2;
                return true;
            }

            fixedUnit = default;
            return false;
        }

        private bool TryParseQuotedString(char quote, out CharSpan span, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            uint start = ptr;

            if ((whitespaceHandling & WhitespaceHandling.ConsumeBefore) != 0) {
                ConsumeWhiteSpaceAndComments();
            }

            if (!HasMoreTokens || data[ptr] != quote) {
                ptr = start;
                span = default;
                return false;
            }

            ptr++;

            uint rangeStart = ptr;
            for (uint i = ptr; i < dataEnd; i++) {
                if (data[i] == quote && data[i - 1] != '\\') {
                    span = new CharSpan(data, (int) rangeStart, (int) i);
                    ptr = i + 1; // step over quote;
                    if ((whitespaceHandling & WhitespaceHandling.ConsumeAfter) != 0) {
                        ConsumeWhiteSpaceAndComments();
                    }

                    return true;
                }
            }

            ptr = start;
            span = default;
            return false;
        }

        public bool TryParseSingleQuotedString(out CharSpan span, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            return TryParseQuotedString('\'', out span, whitespaceHandling);
        }

        public bool TryParseDoubleQuotedString(out CharSpan span, WhitespaceHandling whitespaceHandling = WhitespaceHandling.ConsumeAll) {
            return TryParseQuotedString('"', out span, whitespaceHandling);
        }

        public void ParseFloatAttr(string key, out float floatValue, float defaultValue) {
            if (!TryParseFloatAttr(key, out floatValue)) {
                floatValue = defaultValue;
            }
        }

        /// <summary>
        /// key=value
        /// </summary>
        public bool TryParseFloatAttr(string key, out float floatValue) {
            uint i = ptr;
            uint save = ptr;
            char startChar = key[0];

            while (i != dataEnd) {
                if (data[i] == startChar) {
                    ptr = i;
                    if (Previous != ' ') {
                        i++;
                        continue;
                    }

                    if (TryMatchRange(key) && TryParseCharacter('=')) {
                        if (ptr + 1 >= dataEnd) {
                            i++;
                            continue;
                        }

                        char openQuote = data[ptr];
                        ptr++;
                        if (openQuote != '\'' && openQuote != '"') {
                            openQuote = '\0';
                        }

                        if (!TryParseFloat(out floatValue)) {
                            i++;
                            continue;
                        }

                        if (openQuote != '\0' && !TryParseCharacter(openQuote)) {
                            i++;
                            continue;
                        }

                        ptr = save; // this method doesnt consume the stream
                        return true;
                    }
                }

                i++;
            }

            ptr = save;
            floatValue = 0;
            return false;
        }

        public bool TryParseColorAttr(string key, out Color32 colorValue, char quote = '\0') {
            uint i = ptr;
            uint save = ptr;
            char startChar = key[0];

            while (i != dataEnd) {
                if (data[i] == startChar) {
                    ptr = i;
                    if (Previous != ' ') {
                        i++;
                        continue;
                    }

                    if (TryMatchRange(key) && TryParseCharacter('=')) {
                        char openQuote = data[ptr];

                        if (openQuote != '\'' && openQuote != '"') {
                            openQuote = '\0';
                        }

                        if (!TryParseColorProperty(out colorValue)) {
                            i++;
                            continue;
                        }

                        if (openQuote != '\0' && !TryParseCharacter(openQuote)) {
                            i++;
                            continue;
                        }

                        ptr = save; // this method doesnt consume the stream
                        return true;
                    }
                }

                i++;
            }

            ptr = save;
            colorValue = default;
            return false;
        }

        public void SetCommentMode(CommentMode commentMode) {
            this.commentMode = commentMode;
        }

        public bool ConsumeUntilFound(string str, out CharSpan result) {
            fixed (char* buffer = str) {
                return ConsumeUntilFound(buffer, str.Length, out result);
            }
        }

        public bool ConsumeUntilFound(char* str, int length, out CharSpan result) {
            if (length == 0 || str == null) {
                result = default;
                return false;
            }

            uint max = (uint) (dataEnd - length);
            uint idx = ptr;
            while (idx < max) {
                while (idx < max && data[idx] != str[0]) {
                    idx++;
                }

                if (length == 1) {
                    result = new CharSpan(data, (int) ptr, (int) idx + 1, baseOffset);
                    ptr = (uint) (idx + length);
                    return true;
                }

                bool found = true;

                // todo -- memcmp might be faster, depends on how big length is
                for (int i = 1; i < length; i++) {
                    if (data[idx + i] != str[i]) {
                        found = false;
                        break;
                    }
                }

                if (found) {
                    result = new CharSpan(data, (int) ptr, (int) idx, baseOffset);
                    ptr = idx + (uint) length;
                    return true;
                }

                idx++;
            }

            result = default;
            return false;
        }

        public bool TryMatchRangeFast(char* str, int length) {
            if (ptr + length < dataEnd) {
                bool found = false;
                for (int i = 0; i < length; i++) {
                    if (data[ptr + i] != str[i]) {
                        break;
                    }
                }

                return true;
            }

            return false;
        }

    }

    [Flags]
    public enum WhitespaceHandling {

        None = 0,
        ConsumeBefore = 1 << 0,
        ConsumeAfter = 1 << 1,
        ConsumeAll = ConsumeBefore | ConsumeAfter

    }

    public struct ReflessCharSpan {

        public readonly ushort rangeStart;
        public readonly ushort rangeEnd;

        public int Length => rangeEnd - rangeStart;
        public bool HasValue => Length > 0;

        public ReflessCharSpan(int rangeStart, int rangeEnd) {
            this.rangeStart = (ushort) rangeStart;
            this.rangeEnd = (ushort) rangeEnd;
        }

        public ReflessCharSpan(CharSpan span) {
            this.rangeStart = span.rangeStart;
            this.rangeEnd = span.rangeEnd;
        }

        public ReflessCharSpan(CharStream stream) {
            this.rangeStart = (ushort) stream.Ptr;
            this.rangeEnd = (ushort) stream.End;
        }

        public static string MakeString(in ReflessCharSpan span, char[] data) {
            return new string(data, span.rangeStart, span.rangeEnd - span.rangeStart);
        }

        public string MakeLowerString(char[] data) {
            int length = rangeEnd - rangeStart;

            unsafe {
                char* buffer = stackalloc char[length + 1];
                int idx = 0;
                for (int i = rangeStart; i < rangeEnd; i++) {
                    buffer[idx++] = char.ToLower(data[i]);
                }

                buffer[length] = '\0';
                return new string(buffer);
            }
        }

    }

    public unsafe struct CharSpan : IEquatable<CharSpan> {

        public readonly ushort rangeStart;
        public readonly ushort rangeEnd;
        public readonly int baseOffset;
        public char* data { get; }

        public bool HasValue => Length > 0;

        public int Length => data != null ? rangeEnd - rangeStart : 0;

        // public CharSpan(char[] data, int rangeStart, int rangeEnd) {
        //     fixed (char* charptr = data) {
        //         this.data = charptr;
        //     }
        //
        //     this.rangeStart = (ushort) rangeStart;
        //     this.rangeEnd = (ushort) rangeEnd;
        // }

        public CharSpan(char* data, int rangeStart, int rangeEnd, int baseOffset = 0) {
            this.data = data;
            this.rangeStart = (ushort) rangeStart;
            this.rangeEnd = (ushort) rangeEnd;
            this.baseOffset = baseOffset;
        }

        public CharSpan(CharStream stream) {
            this.data = stream.Data;
            this.rangeStart = (ushort) stream.Ptr;
            this.rangeEnd = (ushort) stream.End;
            this.baseOffset = stream.baseOffset;
        }

        public static bool operator ==(CharSpan a, string b) {
            return StringUtil.EqualsRangeUnsafe(b, a.data, a.rangeStart, a.rangeEnd - a.rangeStart);
        }

        public static bool operator !=(CharSpan a, string b) {
            return !StringUtil.EqualsRangeUnsafe(b, a.data, a.rangeStart, a.rangeEnd - a.rangeStart);
        }

        public static bool operator !=(string b, CharSpan a) {
            return !StringUtil.EqualsRangeUnsafe(b, a.data, a.rangeStart, a.rangeEnd - a.rangeStart);
        }

        public static bool operator ==(string b, CharSpan a) {
            return StringUtil.EqualsRangeUnsafe(b, a.data, a.rangeStart, a.rangeEnd - a.rangeStart);
        }

        public static bool operator ==(CharSpan a, CharSpan b) {
            // this is a value comparison
            int aLen = a.rangeEnd - a.rangeStart;
            int bLen = b.rangeEnd - b.rangeStart;
            if (aLen != bLen) return false;
            for (int i = 0; i < aLen; i++) {
                if (a.data[a.rangeStart + i] != b.data[b.rangeStart + i]) {
                    return false;
                }
            }

            return true;
        }

        public bool EqualsIgnoreCase(string str) {
            if (rangeEnd - rangeStart != str.Length) return false;

            fixed (char* s = str) {
                int idx = 0;
                for (int i = rangeStart; i < rangeEnd; i++) {
                    char strChar = s[idx++];
                    char dataChar = data[i];

                    char c1 = strChar >= 'a' && strChar <= 'z' ? char.ToLower(strChar) : strChar;
                    char c2 = dataChar >= 'a' && dataChar <= 'z' ? char.ToLower(dataChar) : dataChar;

                    if (c1 != c2) {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator !=(CharSpan a, CharSpan b) {
            return !(a == b);
        }

        public bool Equals(CharSpan other) {
            return this == other;
        }

        public override bool Equals(object obj) {
            return obj is CharSpan other && Equals(other);
        }

        public override int GetHashCode() {
#if WIN32
            int hash1 = (5381<<16) + 5381;
#else
            int hash1 = 5381;
#endif
            int hash2 = hash1;

#if WIN32
                    // 32 bit machines.
                    int* pint = (int *)src;
                    int len = this.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len -= 4;
                    }
 
                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }
#else
            int c;
            char* s = &data[rangeStart];
            while ((c = s[0]) != 0) {
                hash1 = ((hash1 << 5) + hash1) ^ c;
                c = s[1];
                if (c == 0)
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ c;
                s += 2;
            }
#endif
            return hash1 + (hash2 * 1566083941);
        }

        public override string ToString() {
            return new string(data, rangeStart, rangeEnd - rangeStart);
        }

        public string ToLowerString() {
            int length = rangeEnd - rangeStart;

            char* buffer = stackalloc char[length + 1];
            int idx = 0;
            for (int i = rangeStart; i < rangeEnd; i++) {
                buffer[idx++] = char.ToLower(data[i]);
            }

            buffer[length] = '\0';
            return new string(buffer);
        }

        public void ToLower(char[] buffer) {
            int idx = 0;

            for (int i = rangeStart; i < rangeEnd; i++) {
                buffer[idx++] = char.ToLower(data[i]);
            }
        }

        public CharSpan Trim() {
            int start = rangeStart;
            int end = rangeEnd;
            for (int i = rangeStart; i < rangeEnd; i++) {
                if (char.IsWhiteSpace(data[i])) {
                    start++;
                }
                else {
                    break;
                }
            }

            for (int i = rangeEnd - 1; i >= start; i--) {
                if (char.IsWhiteSpace(data[i])) {
                    end--;
                }
                else {
                    break;
                }
            }

            return new CharSpan(data, start, end);
        }

        public int GetLineNumber() {
            int line = 0;
            for (int i = 0; i < rangeStart; i++) {
                if (data[i] == '\n') line++;
            }

            return line;
        }

        public int GetEndLineNumber() {
            int line = 0;
            for (int i = 0; i < rangeEnd; i++) {
                if (data[i] == '\n') line++;
            }

            return line;
        }

        public ReflessCharSpan ToRefless() {
            return new ReflessCharSpan(this);
        }

        public char this[int i] => data[i];

        public int IndexOf(char c) {
            for (int i = rangeStart; i < rangeEnd; i++) {
                if (data[i] == c) {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(char c) {
            for (int i = rangeEnd - 1; i >= rangeStart; i--) {
                if (data[i] == c) {
                    return i;
                }
            }

            return -1;
        }

        public RangeInt GetContentRange() {
            return new RangeInt(rangeStart, rangeEnd - rangeEnd);
        }

        public bool TryParseColor(out Color32 color) {
            return new CharStream(this).TryParseColorProperty(out color);
        }

        public LineInfo GetLineInfo() {
            int line = 1; // start counting at line 1
            int x = 0;

            for (int i = 0; i < rangeStart; i++) {
                if (data[i] == '\n') {
                    line++;
                    x = i + 1;
                }
            }

            int col = 1;
            for (int i = x; i < rangeStart - 1; i++) {
                col++;
            }

            return new LineInfo(line, col);
        }

        public bool Contains(char c) {
            for (int i = rangeStart; i < rangeEnd; i++) {
                char test = data[i];
                if (test == c) {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(string str) {
            char c = str[0];
            for (int i = rangeStart; i < rangeEnd; i++) {
                char test = data[i];
                if (test == c) {
                    if (StringUtil.EqualsRangeUnsafe(str, data, i, str.Length)) {
                        return true;
                    }
                }

                if (i + str.Length >= rangeEnd) {
                    return false;
                }
            }

            return false;
        }

        public bool StartsWith(string str) {
            if (HasValue || Length < str.Length) {
                return false;
            }

            return StringUtil.EqualsRangeUnsafe(str, data, rangeStart, str.Length);
        }

        public bool StartsWith(char c, bool trim = true) {
            if (!trim) {
                return HasValue && data[rangeStart] == c;
            }

            for (int i = rangeStart; i < rangeEnd; i++) {
                char test = data[i];

                if (char.IsWhiteSpace(test)) {
                    continue;
                }

                if (test == c) {
                    return true;
                }

                return false;
            }

            return false;
        }

        public bool EndsWith(char c, bool trim = true) {
            if (!trim) {
                return HasValue && data[rangeEnd - 1] == c;
            }

            for (int i = rangeEnd - 1; i != rangeStart; i--) {
                char test = data[i];

                if (char.IsWhiteSpace(test)) {
                    continue;
                }

                if (test == c) {
                    return true;
                }

                return false;
            }

            return false;
        }

        public CharSpan Substring(int length) {
            return new CharSpan(data, rangeStart + length, rangeEnd);
        }

    }

}