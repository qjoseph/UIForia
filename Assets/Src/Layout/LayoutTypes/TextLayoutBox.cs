using System.Collections.Generic;
using Src.Systems;
using Src.Text;
using Src.Util;
using TMPro;
using UnityEngine;

namespace Src.Layout.LayoutTypes {

    public class TextLayoutBox : LayoutBox {

        public TextLayoutBox(LayoutSystem layoutSystem, UIElement element)
            : base(layoutSystem, element) { }

        protected override float ComputeContentWidth() {
            TextInfo textInfo = ((UITextElement) element).textInfo;
            List<LineInfo> lineInfos = RunLayout(textInfo, float.MaxValue);

            ((UITextElement) element).textInfo = textInfo;

            float maxWidth = 0;
            for (int i = 0; i < lineInfos.Count; i++) {
                maxWidth = Mathf.Max(maxWidth, lineInfos[i].width);
            }

            ListPool<LineInfo>.Release(ref lineInfos);
            
            return maxWidth;
        }

        protected override float ComputeContentHeight(float width) {
            TextInfo textInfo = ((UITextElement) element).textInfo;
            List<LineInfo> lineInfos = RunLayout(textInfo, width);
            LineInfo lastLine = lineInfos[lineInfos.Count - 1];
            ListPool<LineInfo>.Release(ref lineInfos);
            return -lastLine.position.y + lastLine.Height;
        }

        public override void RunLayout() {
            TextInfo textInfo = ((UITextElement) element).textInfo;
            List<LineInfo> lineInfos = RunLayout(textInfo, allocatedWidth);
            LineInfo lastLine = lineInfos[lineInfos.Count - 1];

            textInfo.lineInfos = ArrayPool<LineInfo>.CopyFromList(lineInfos);
            textInfo.lineCount = lineInfos.Count;
            ((UITextElement) element).textInfo = textInfo;

            float maxWidth = 0;
            for (int i = 0; i < lineInfos.Count; i++) {
                maxWidth = Mathf.Max(maxWidth, lineInfos[i].width);
            }

            actualWidth = maxWidth;
            actualHeight = -lastLine.position.y + lastLine.Height;

            ApplyTextAlignment(allocatedWidth, textInfo, style.TextAlignment);

            ListPool<LineInfo>.Release(ref lineInfos);
        }

        // todo -- when starting a new line, if the last line has spaces as the final characters, consider stripping them

        private static List<LineInfo> RunLayout(TextInfo textInfo, float width) {
            float lineOffset = 0;
            SpanInfo spanInfo = textInfo.spanInfos[0];

            LineInfo currentLine = new LineInfo();
            WordInfo[] wordInfos = textInfo.wordInfos;
            List<LineInfo> lineInfos = ListPool<LineInfo>.Get();

            TMP_FontAsset font = spanInfo.font ? spanInfo.font : TMP_FontAsset.defaultFontAsset;
            float lineGap = font.fontInfo.LineHeight - (font.fontInfo.Ascender - font.fontInfo.Descender);
            float baseScale = (spanInfo.fontSize / font.fontInfo.PointSize * font.fontInfo.Scale);
            float lineHeight = (font.fontInfo.LineHeight + lineGap) * baseScale;
            // todo -- might want to use an optional 'lineHeight' setting instead of just computing the line height

            for (int w = 0; w < textInfo.wordCount; w++) {
                WordInfo currentWord = wordInfos[w];

                if (currentWord.isNewLine) {
                    lineInfos.Add(currentLine);
                    lineOffset -= (currentLine.maxDescender + textInfo.charInfos[currentWord.startChar + currentWord.VisibleCharCount - 1].ascender + lineGap) * baseScale;
                    currentLine = new LineInfo();
                    currentLine.position = new Vector2(currentLine.position.x, lineOffset);
                    currentLine.wordStart = w + 1;
                    continue;
                }

                if (currentWord.characterSize > width + 0.01f) {
                    // we had words in this line already
                    // finish the line and start a new one
                    // line offset needs to to be bumped
                    if (currentLine.wordCount > 0) {
                        lineInfos.Add(currentLine);
                        lineOffset -= -currentLine.maxDescender + textInfo.charInfos[currentWord.startChar + currentWord.VisibleCharCount - 1].ascender + (lineGap) * baseScale;
                    }

                    currentLine = new LineInfo();
                    currentLine.position = new Vector2(0, lineOffset);
                    currentLine.wordStart = w;
                    currentLine.wordCount = 1;
                    currentLine.maxAscender = currentWord.ascender;
                    currentLine.maxDescender = currentWord.descender;
                    currentLine.width = currentWord.size.x;
                    lineInfos.Add(currentLine);

                    lineOffset -= -currentLine.maxDescender + textInfo.charInfos[currentWord.startChar + currentWord.VisibleCharCount - 1].ascender + (lineGap) * baseScale;
                    currentLine = new LineInfo();
                    currentLine.wordStart = w + 1;
                    currentLine.position = new Vector2(currentLine.position.x, lineOffset);
                }

                else if (currentLine.width + currentWord.size.x > width + 0.01f) {
                    int s = (int) (currentLine.width + currentWord.size.x);
                    // characters fit but space does not, strip spaces and start new line w/ next word
                    if (currentLine.width + currentWord.characterSize < width + 0.01f) {
                        currentLine.wordCount++;

                        if (currentLine.maxAscender < currentWord.ascender) currentLine.maxAscender = currentWord.ascender;
                        if (currentLine.maxDescender > currentWord.descender) currentLine.maxDescender = currentWord.descender;
                        currentLine.width += currentWord.characterSize;
                        lineInfos.Add(currentLine);

                        lineOffset -= -currentLine.maxDescender + textInfo.charInfos[currentWord.startChar + currentWord.VisibleCharCount - 1].ascender + (lineGap) * baseScale;

                        currentLine = new LineInfo();
                        currentLine.position = new Vector2(currentLine.position.x, lineOffset);
                        currentLine.wordStart = w + 1;
                        continue;
                    }

                    lineInfos.Add(currentLine);
                    lineOffset -= -currentLine.maxDescender + textInfo.charInfos[currentWord.startChar + currentWord.VisibleCharCount - 1].ascender + (lineGap) * baseScale;
                    currentLine = new LineInfo();
                    currentLine.position = new Vector2(currentLine.position.x, lineOffset);
                    currentLine.wordStart = w;
                    currentLine.wordCount = 1;
                    currentLine.width = currentWord.size.x;
                    currentLine.maxAscender = currentWord.ascender;
                    currentLine.maxDescender = currentWord.descender;
                }

                else {
                    currentLine.wordCount++;

                    if (currentLine.maxAscender < currentWord.maxCharTop) currentLine.maxAscender = currentWord.maxCharTop;
                    if (currentLine.maxDescender > currentWord.minCharBottom) currentLine.maxDescender = currentWord.minCharBottom;

                    currentLine.width += currentWord.xAdvance;
                }
            }

            if (currentLine.wordCount > 0) {
                lineInfos.Add(currentLine);
            }

            return lineInfos;
        }

        private static void ApplyTextAlignment(float allocatedWidth, TextInfo textInfo, TextUtil.TextAlignment alignment) {
            LineInfo[] lineInfos = textInfo.lineInfos;

            // find max line width
            // if line width < allocated width use allocated width

            float lineWidth = allocatedWidth;
            for (int i = 0; i < textInfo.lineCount; i++) {
                lineWidth = Mathf.Max(lineWidth, lineInfos[i].width);
            }

            switch (alignment) {
                case TextUtil.TextAlignment.Center:

                    for (int i = 0; i < textInfo.lineCount; i++) {
                        float offset = (lineWidth - lineInfos[i].width) * 0.5f;
                        if (offset <= 0) break;
                        lineInfos[i].position = new Vector2(offset, lineInfos[i].position.y);
                    }

                    break;
                case TextUtil.TextAlignment.Right:

                    for (int i = 0; i < textInfo.lineCount; i++) {
                        float offset = (lineWidth - lineInfos[i].width);
                        lineInfos[i].position = new Vector2(offset, lineInfos[i].position.y);
                    }

                    break;
            }
        }

        public void OnTextContentUpdated() {
            RequestOwnSizeChangedLayout();
        }

    }

}