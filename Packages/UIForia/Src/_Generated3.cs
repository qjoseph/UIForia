using UIForia.Layout;
using UIForia.Layout.LayoutTypes;
using UIForia.Bindings.StyleBindings;
using System.Collections.Generic;
using UnityEngine;
using UIForia.Util;
using UIForia.Text;
using FontStyle = UIForia.Text.FontStyle;
using TextAlignment = UIForia.Text.TextAlignment;

// Do not edit this file. See CodeGen.cs instead.

namespace UIForia.Rendering {

    public static class DefaultStyleValues_Generated {

		public const Visibility Visibility = UIForia.Rendering.Visibility.Visible;
		public const float Opacity = 1f;
		public static readonly CursorStyle Cursor = default(CursorStyle);
		public static readonly string Painter = "";
		public const Overflow OverflowX = UIForia.Rendering.Overflow.Visible;
		public const Overflow OverflowY = UIForia.Rendering.Overflow.Visible;
		public const ClipBehavior ClipBehavior = UIForia.Layout.ClipBehavior.Normal;
		public const ClipBounds ClipBounds = UIForia.Rendering.ClipBounds.BorderBox;
		public static readonly Color BackgroundColor = new Color(-1f, -1f, -1f, -1f);
		public static readonly Color BackgroundTint = new Color(-1f, -1f, -1f, -1f);
		public static readonly UIFixedLength BackgroundImageOffsetX = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BackgroundImageOffsetY = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public const float BackgroundImageScaleX = 1f;
		public const float BackgroundImageScaleY = 1f;
		public const float BackgroundImageTileX = 1f;
		public const float BackgroundImageTileY = 1f;
		public const float BackgroundImageRotation = 0f;
		public static readonly Texture2D BackgroundImage = default(Texture2D);
		public const BackgroundFit BackgroundFit = UIForia.Rendering.BackgroundFit.Fill;
		public static readonly Color BorderColorTop = new Color(-1f, -1f, -1f, -1f);
		public static readonly Color BorderColorRight = new Color(-1f, -1f, -1f, -1f);
		public static readonly Color BorderColorBottom = new Color(-1f, -1f, -1f, -1f);
		public static readonly Color BorderColorLeft = new Color(-1f, -1f, -1f, -1f);
		public static readonly UIFixedLength CornerBevelTopLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength CornerBevelTopRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength CornerBevelBottomRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength CornerBevelBottomLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public const int FlexItemGrow = 0;
		public const int FlexItemShrink = 0;
		public const LayoutDirection FlexLayoutDirection = UIForia.Layout.LayoutDirection.Vertical;
		public const LayoutWrap FlexLayoutWrap = UIForia.Layout.LayoutWrap.None;
		public const SpaceDistribution FlexLayoutMainAxisAlignment = UIForia.Layout.SpaceDistribution.BeforeContent;
		public const CrossAxisAlignment FlexLayoutCrossAxisAlignment = UIForia.Layout.CrossAxisAlignment.Start;
		public static readonly GridItemPlacement GridItemX = new GridItemPlacement(-1);
		public static readonly GridItemPlacement GridItemY = new GridItemPlacement(-1);
		public static readonly GridItemPlacement GridItemWidth = new GridItemPlacement(1);
		public static readonly GridItemPlacement GridItemHeight = new GridItemPlacement(1);
		public const LayoutDirection GridLayoutDirection = UIForia.Layout.LayoutDirection.Horizontal;
		public const GridLayoutDensity GridLayoutDensity = UIForia.Layout.GridLayoutDensity.Sparse;
		public static readonly IReadOnlyList<UIForia.Layout.LayoutTypes.GridTrackSize> GridLayoutColTemplate = ListPool<GridTrackSize>.Empty;
		public static readonly IReadOnlyList<UIForia.Layout.LayoutTypes.GridTrackSize> GridLayoutRowTemplate = ListPool<GridTrackSize>.Empty;
		public static readonly IReadOnlyList<UIForia.Layout.LayoutTypes.GridTrackSize> GridLayoutColAutoSize = new List<GridTrackSize>() {GridTrackSize.MaxContent};
		public static readonly IReadOnlyList<UIForia.Layout.LayoutTypes.GridTrackSize> GridLayoutRowAutoSize = new List<GridTrackSize>() {GridTrackSize.MaxContent};
		public const float GridLayoutColGap = 0f;
		public const float GridLayoutRowGap = 0f;
		public const GridAxisAlignment GridLayoutColAlignment = UIForia.Layout.GridAxisAlignment.Grow;
		public const GridAxisAlignment GridLayoutRowAlignment = UIForia.Layout.GridAxisAlignment.Grow;
		public const float AlignItemsHorizontal = 0f;
		public const float AlignItemsVertical = 0f;
		public const LayoutFit FitItemsVertical = UIForia.Layout.LayoutFit.Unset;
		public const LayoutFit FitItemsHorizontal = UIForia.Layout.LayoutFit.Unset;
		public const SpaceDistribution DistributeExtraSpaceHorizontal = UIForia.Layout.SpaceDistribution.AfterContent;
		public const SpaceDistribution DistributeExtraSpaceVertical = UIForia.Layout.SpaceDistribution.AfterContent;
		public const float RadialLayoutStartAngle = 0f;
		public const float RadialLayoutEndAngle = 360f;
		public static readonly UIFixedLength RadialLayoutRadius = new UIFixedLength(0.5f, UIFixedUnit.Percent);
		public const AlignmentDirection AlignmentDirectionX = UIForia.Layout.AlignmentDirection.Start;
		public const AlignmentDirection AlignmentDirectionY = UIForia.Layout.AlignmentDirection.Start;
		public const AlignmentBehavior AlignmentBehaviorX = UIForia.Layout.AlignmentBehavior.Default;
		public const AlignmentBehavior AlignmentBehaviorY = UIForia.Layout.AlignmentBehavior.Default;
		public static readonly OffsetMeasurement AlignmentOriginX = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public static readonly OffsetMeasurement AlignmentOriginY = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public static readonly OffsetMeasurement AlignmentOffsetX = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public static readonly OffsetMeasurement AlignmentOffsetY = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public const LayoutFit LayoutFitHorizontal = UIForia.Layout.LayoutFit.Unset;
		public const LayoutFit LayoutFitVertical = UIForia.Layout.LayoutFit.Unset;
		public static readonly UIMeasurement MinWidth = new UIMeasurement(0f, UIMeasurementUnit.Pixel);
		public static readonly UIMeasurement MaxWidth = new UIMeasurement(3.402823E+38f, UIMeasurementUnit.Pixel);
		public static readonly UIMeasurement PreferredWidth = new UIMeasurement(1f, UIMeasurementUnit.Content);
		public static readonly UIMeasurement MinHeight = new UIMeasurement(0f, UIMeasurementUnit.Pixel);
		public static readonly UIMeasurement MaxHeight = new UIMeasurement(3.402823E+38f, UIMeasurementUnit.Pixel);
		public static readonly UIMeasurement PreferredHeight = new UIMeasurement(1f, UIMeasurementUnit.Content);
		public static readonly UIFixedLength MarginTop = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength MarginRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength MarginBottom = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength MarginLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderTop = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderBottom = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderRadiusTopLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderRadiusTopRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderRadiusBottomRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength BorderRadiusBottomLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength PaddingTop = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength PaddingRight = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength PaddingBottom = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength PaddingLeft = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly Color TextColor = new Color(0f, 0f, 0f, 1f);
		public static readonly FontAsset TextFontAsset = FontAsset.defaultFontAsset;
		public static readonly UIFixedLength TextFontSize = new UIFixedLength(18f, UIFixedUnit.Pixel);
		public const FontStyle TextFontStyle = UIForia.Text.FontStyle.Normal;
		public const TextAlignment TextAlignment = UIForia.Text.TextAlignment.Left;
		public const float TextOutlineWidth = 0f;
		public static readonly Color TextOutlineColor = new Color(0f, 0f, 0f, 1f);
		public static readonly Color CaretColor = new Color(0f, 0f, 0f, 1f);
		public static readonly Color SelectionBackgroundColor = new Color(0.7215686f, 1f, 1f, 1f);
		public static readonly Color SelectionTextColor = new Color(0f, 0f, 0f, 1f);
		public const float TextOutlineSoftness = 0f;
		public static readonly Color TextGlowColor = new Color(-1f, -1f, -1f, -1f);
		public const float TextGlowOffset = 0f;
		public const float TextGlowInner = 0f;
		public const float TextGlowOuter = 0f;
		public const float TextGlowPower = 0f;
		public static readonly Color TextUnderlayColor = new Color(-1f, -1f, -1f, -1f);
		public const float TextUnderlayX = 0f;
		public const float TextUnderlayY = 0f;
		public const float TextUnderlayDilate = 0f;
		public const float TextUnderlaySoftness = 0f;
		public const float TextFaceDilate = 0f;
		public const UnderlayType TextUnderlayType = UIForia.Rendering.UnderlayType.Unset;
		public const TextTransform TextTransform = UIForia.Text.TextTransform.None;
		public const WhitespaceMode TextWhitespaceMode = UIForia.Text.WhitespaceMode.CollapseWhitespaceAndTrim;
		public static readonly OffsetMeasurement TransformPositionX = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public static readonly OffsetMeasurement TransformPositionY = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public static readonly UIFixedLength TransformPivotX = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public static readonly UIFixedLength TransformPivotY = new UIFixedLength(0f, UIFixedUnit.Pixel);
		public const float TransformScaleX = 1f;
		public const float TransformScaleY = 1f;
		public const float TransformRotation = 0f;
		public const TransformBehavior TransformBehaviorX = UIForia.Rendering.TransformBehavior.LayoutOffset;
		public const TransformBehavior TransformBehaviorY = UIForia.Rendering.TransformBehavior.LayoutOffset;
		public const LayoutType LayoutType = UIForia.Layout.LayoutType.Flex;
		public const LayoutBehavior LayoutBehavior = UIForia.Layout.LayoutBehavior.Normal;
		public const int ZIndex = 0;
		public const int RenderLayerOffset = 0;
		public const RenderLayer RenderLayer = UIForia.Rendering.RenderLayer.Default;
		public const int Layer = 0;
		public static readonly Color ShadowColor = new Color(-1f, -1f, -1f, -1f);
		public static readonly Color ShadowTint = new Color(-1f, -1f, -1f, -1f);
		public static readonly OffsetMeasurement ShadowOffsetX = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public static readonly OffsetMeasurement ShadowOffsetY = new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel);
		public const float ShadowSizeX = 0f;
		public const float ShadowSizeY = 0f;
		public const float ShadowIntensity = 10f;
		public const float ShadowOpacity = 1f;
		public static StyleProperty GetPropertyValue(StylePropertyId propertyId) {

			switch(propertyId) {
				case StylePropertyId.Visibility:
					 return new StyleProperty(StylePropertyId.Visibility, (int)UIForia.Rendering.Visibility.Visible);
				case StylePropertyId.Opacity:
					 return new StyleProperty(StylePropertyId.Opacity, 1f);
				case StylePropertyId.Cursor:
					 return new StyleProperty(StylePropertyId.Cursor, default(CursorStyle));
				case StylePropertyId.Painter:
					 return new StyleProperty(StylePropertyId.Painter, "");
				case StylePropertyId.OverflowX:
					 return new StyleProperty(StylePropertyId.OverflowX, (int)UIForia.Rendering.Overflow.Visible);
				case StylePropertyId.OverflowY:
					 return new StyleProperty(StylePropertyId.OverflowY, (int)UIForia.Rendering.Overflow.Visible);
				case StylePropertyId.ClipBehavior:
					 return new StyleProperty(StylePropertyId.ClipBehavior, (int)UIForia.Layout.ClipBehavior.Normal);
				case StylePropertyId.ClipBounds:
					 return new StyleProperty(StylePropertyId.ClipBounds, (int)UIForia.Rendering.ClipBounds.BorderBox);
				case StylePropertyId.BackgroundColor:
					 return new StyleProperty(StylePropertyId.BackgroundColor, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.BackgroundTint:
					 return new StyleProperty(StylePropertyId.BackgroundTint, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.BackgroundImageOffsetX:
					 return new StyleProperty(StylePropertyId.BackgroundImageOffsetX, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BackgroundImageOffsetY:
					 return new StyleProperty(StylePropertyId.BackgroundImageOffsetY, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BackgroundImageScaleX:
					 return new StyleProperty(StylePropertyId.BackgroundImageScaleX, 1f);
				case StylePropertyId.BackgroundImageScaleY:
					 return new StyleProperty(StylePropertyId.BackgroundImageScaleY, 1f);
				case StylePropertyId.BackgroundImageTileX:
					 return new StyleProperty(StylePropertyId.BackgroundImageTileX, 1f);
				case StylePropertyId.BackgroundImageTileY:
					 return new StyleProperty(StylePropertyId.BackgroundImageTileY, 1f);
				case StylePropertyId.BackgroundImageRotation:
					 return new StyleProperty(StylePropertyId.BackgroundImageRotation, 0f);
				case StylePropertyId.BackgroundImage:
					 return new StyleProperty(StylePropertyId.BackgroundImage, default(Texture2D));
				case StylePropertyId.BackgroundFit:
					 return new StyleProperty(StylePropertyId.BackgroundFit, (int)UIForia.Rendering.BackgroundFit.Fill);
				case StylePropertyId.BorderColorTop:
					 return new StyleProperty(StylePropertyId.BorderColorTop, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.BorderColorRight:
					 return new StyleProperty(StylePropertyId.BorderColorRight, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.BorderColorBottom:
					 return new StyleProperty(StylePropertyId.BorderColorBottom, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.BorderColorLeft:
					 return new StyleProperty(StylePropertyId.BorderColorLeft, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.CornerBevelTopLeft:
					 return new StyleProperty(StylePropertyId.CornerBevelTopLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.CornerBevelTopRight:
					 return new StyleProperty(StylePropertyId.CornerBevelTopRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.CornerBevelBottomRight:
					 return new StyleProperty(StylePropertyId.CornerBevelBottomRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.CornerBevelBottomLeft:
					 return new StyleProperty(StylePropertyId.CornerBevelBottomLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.FlexItemGrow:
					 return new StyleProperty(StylePropertyId.FlexItemGrow, 0);
				case StylePropertyId.FlexItemShrink:
					 return new StyleProperty(StylePropertyId.FlexItemShrink, 0);
				case StylePropertyId.FlexLayoutDirection:
					 return new StyleProperty(StylePropertyId.FlexLayoutDirection, (int)UIForia.Layout.LayoutDirection.Vertical);
				case StylePropertyId.FlexLayoutWrap:
					 return new StyleProperty(StylePropertyId.FlexLayoutWrap, (int)UIForia.Layout.LayoutWrap.None);
				case StylePropertyId.FlexLayoutMainAxisAlignment:
					 return new StyleProperty(StylePropertyId.FlexLayoutMainAxisAlignment, (int)UIForia.Layout.SpaceDistribution.BeforeContent);
				case StylePropertyId.FlexLayoutCrossAxisAlignment:
					 return new StyleProperty(StylePropertyId.FlexLayoutCrossAxisAlignment, (int)UIForia.Layout.CrossAxisAlignment.Start);
				case StylePropertyId.GridItemX:
					 return new StyleProperty(StylePropertyId.GridItemX, new GridItemPlacement(-1));
				case StylePropertyId.GridItemY:
					 return new StyleProperty(StylePropertyId.GridItemY, new GridItemPlacement(-1));
				case StylePropertyId.GridItemWidth:
					 return new StyleProperty(StylePropertyId.GridItemWidth, new GridItemPlacement(1));
				case StylePropertyId.GridItemHeight:
					 return new StyleProperty(StylePropertyId.GridItemHeight, new GridItemPlacement(1));
				case StylePropertyId.GridLayoutDirection:
					 return new StyleProperty(StylePropertyId.GridLayoutDirection, (int)UIForia.Layout.LayoutDirection.Horizontal);
				case StylePropertyId.GridLayoutDensity:
					 return new StyleProperty(StylePropertyId.GridLayoutDensity, (int)UIForia.Layout.GridLayoutDensity.Sparse);
				case StylePropertyId.GridLayoutColTemplate:
					 return new StyleProperty(StylePropertyId.GridLayoutColTemplate, ListPool<GridTrackSize>.Empty);
				case StylePropertyId.GridLayoutRowTemplate:
					 return new StyleProperty(StylePropertyId.GridLayoutRowTemplate, ListPool<GridTrackSize>.Empty);
				case StylePropertyId.GridLayoutColAutoSize:
					 return new StyleProperty(StylePropertyId.GridLayoutColAutoSize, new List<GridTrackSize>() {GridTrackSize.MaxContent});
				case StylePropertyId.GridLayoutRowAutoSize:
					 return new StyleProperty(StylePropertyId.GridLayoutRowAutoSize, new List<GridTrackSize>() {GridTrackSize.MaxContent});
				case StylePropertyId.GridLayoutColGap:
					 return new StyleProperty(StylePropertyId.GridLayoutColGap, 0f);
				case StylePropertyId.GridLayoutRowGap:
					 return new StyleProperty(StylePropertyId.GridLayoutRowGap, 0f);
				case StylePropertyId.GridLayoutColAlignment:
					 return new StyleProperty(StylePropertyId.GridLayoutColAlignment, (int)UIForia.Layout.GridAxisAlignment.Grow);
				case StylePropertyId.GridLayoutRowAlignment:
					 return new StyleProperty(StylePropertyId.GridLayoutRowAlignment, (int)UIForia.Layout.GridAxisAlignment.Grow);
				case StylePropertyId.AlignItemsHorizontal:
					 return new StyleProperty(StylePropertyId.AlignItemsHorizontal, 0f);
				case StylePropertyId.AlignItemsVertical:
					 return new StyleProperty(StylePropertyId.AlignItemsVertical, 0f);
				case StylePropertyId.FitItemsVertical:
					 return new StyleProperty(StylePropertyId.FitItemsVertical, (int)UIForia.Layout.LayoutFit.Unset);
				case StylePropertyId.FitItemsHorizontal:
					 return new StyleProperty(StylePropertyId.FitItemsHorizontal, (int)UIForia.Layout.LayoutFit.Unset);
				case StylePropertyId.DistributeExtraSpaceHorizontal:
					 return new StyleProperty(StylePropertyId.DistributeExtraSpaceHorizontal, (int)UIForia.Layout.SpaceDistribution.AfterContent);
				case StylePropertyId.DistributeExtraSpaceVertical:
					 return new StyleProperty(StylePropertyId.DistributeExtraSpaceVertical, (int)UIForia.Layout.SpaceDistribution.AfterContent);
				case StylePropertyId.RadialLayoutStartAngle:
					 return new StyleProperty(StylePropertyId.RadialLayoutStartAngle, 0f);
				case StylePropertyId.RadialLayoutEndAngle:
					 return new StyleProperty(StylePropertyId.RadialLayoutEndAngle, 360f);
				case StylePropertyId.RadialLayoutRadius:
					 return new StyleProperty(StylePropertyId.RadialLayoutRadius, new UIFixedLength(0.5f, UIFixedUnit.Percent));
				case StylePropertyId.AlignmentDirectionX:
					 return new StyleProperty(StylePropertyId.AlignmentDirectionX, (int)UIForia.Layout.AlignmentDirection.Start);
				case StylePropertyId.AlignmentDirectionY:
					 return new StyleProperty(StylePropertyId.AlignmentDirectionY, (int)UIForia.Layout.AlignmentDirection.Start);
				case StylePropertyId.AlignmentBehaviorX:
					 return new StyleProperty(StylePropertyId.AlignmentBehaviorX, (int)UIForia.Layout.AlignmentBehavior.Default);
				case StylePropertyId.AlignmentBehaviorY:
					 return new StyleProperty(StylePropertyId.AlignmentBehaviorY, (int)UIForia.Layout.AlignmentBehavior.Default);
				case StylePropertyId.AlignmentOriginX:
					 return new StyleProperty(StylePropertyId.AlignmentOriginX, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.AlignmentOriginY:
					 return new StyleProperty(StylePropertyId.AlignmentOriginY, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.AlignmentOffsetX:
					 return new StyleProperty(StylePropertyId.AlignmentOffsetX, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.AlignmentOffsetY:
					 return new StyleProperty(StylePropertyId.AlignmentOffsetY, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.LayoutFitHorizontal:
					 return new StyleProperty(StylePropertyId.LayoutFitHorizontal, (int)UIForia.Layout.LayoutFit.Unset);
				case StylePropertyId.LayoutFitVertical:
					 return new StyleProperty(StylePropertyId.LayoutFitVertical, (int)UIForia.Layout.LayoutFit.Unset);
				case StylePropertyId.MinWidth:
					 return new StyleProperty(StylePropertyId.MinWidth, new UIMeasurement(0f, UIMeasurementUnit.Pixel));
				case StylePropertyId.MaxWidth:
					 return new StyleProperty(StylePropertyId.MaxWidth, new UIMeasurement(3.402823E+38f, UIMeasurementUnit.Pixel));
				case StylePropertyId.PreferredWidth:
					 return new StyleProperty(StylePropertyId.PreferredWidth, new UIMeasurement(1f, UIMeasurementUnit.Content));
				case StylePropertyId.MinHeight:
					 return new StyleProperty(StylePropertyId.MinHeight, new UIMeasurement(0f, UIMeasurementUnit.Pixel));
				case StylePropertyId.MaxHeight:
					 return new StyleProperty(StylePropertyId.MaxHeight, new UIMeasurement(3.402823E+38f, UIMeasurementUnit.Pixel));
				case StylePropertyId.PreferredHeight:
					 return new StyleProperty(StylePropertyId.PreferredHeight, new UIMeasurement(1f, UIMeasurementUnit.Content));
				case StylePropertyId.MarginTop:
					 return new StyleProperty(StylePropertyId.MarginTop, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.MarginRight:
					 return new StyleProperty(StylePropertyId.MarginRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.MarginBottom:
					 return new StyleProperty(StylePropertyId.MarginBottom, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.MarginLeft:
					 return new StyleProperty(StylePropertyId.MarginLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderTop:
					 return new StyleProperty(StylePropertyId.BorderTop, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderRight:
					 return new StyleProperty(StylePropertyId.BorderRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderBottom:
					 return new StyleProperty(StylePropertyId.BorderBottom, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderLeft:
					 return new StyleProperty(StylePropertyId.BorderLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderRadiusTopLeft:
					 return new StyleProperty(StylePropertyId.BorderRadiusTopLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderRadiusTopRight:
					 return new StyleProperty(StylePropertyId.BorderRadiusTopRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderRadiusBottomRight:
					 return new StyleProperty(StylePropertyId.BorderRadiusBottomRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.BorderRadiusBottomLeft:
					 return new StyleProperty(StylePropertyId.BorderRadiusBottomLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.PaddingTop:
					 return new StyleProperty(StylePropertyId.PaddingTop, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.PaddingRight:
					 return new StyleProperty(StylePropertyId.PaddingRight, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.PaddingBottom:
					 return new StyleProperty(StylePropertyId.PaddingBottom, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.PaddingLeft:
					 return new StyleProperty(StylePropertyId.PaddingLeft, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.TextColor:
					 return new StyleProperty(StylePropertyId.TextColor, new Color(0f, 0f, 0f, 1f));
				case StylePropertyId.TextFontAsset:
					 return new StyleProperty(StylePropertyId.TextFontAsset, FontAsset.defaultFontAsset);
				case StylePropertyId.TextFontSize:
					 return new StyleProperty(StylePropertyId.TextFontSize, new UIFixedLength(18f, UIFixedUnit.Pixel));
				case StylePropertyId.TextFontStyle:
					 return new StyleProperty(StylePropertyId.TextFontStyle, (int)UIForia.Text.FontStyle.Normal);
				case StylePropertyId.TextAlignment:
					 return new StyleProperty(StylePropertyId.TextAlignment, (int)UIForia.Text.TextAlignment.Left);
				case StylePropertyId.TextOutlineWidth:
					 return new StyleProperty(StylePropertyId.TextOutlineWidth, 0f);
				case StylePropertyId.TextOutlineColor:
					 return new StyleProperty(StylePropertyId.TextOutlineColor, new Color(0f, 0f, 0f, 1f));
				case StylePropertyId.CaretColor:
					 return new StyleProperty(StylePropertyId.CaretColor, new Color(0f, 0f, 0f, 1f));
				case StylePropertyId.SelectionBackgroundColor:
					 return new StyleProperty(StylePropertyId.SelectionBackgroundColor, new Color(0.7215686f, 1f, 1f, 1f));
				case StylePropertyId.SelectionTextColor:
					 return new StyleProperty(StylePropertyId.SelectionTextColor, new Color(0f, 0f, 0f, 1f));
				case StylePropertyId.TextOutlineSoftness:
					 return new StyleProperty(StylePropertyId.TextOutlineSoftness, 0f);
				case StylePropertyId.TextGlowColor:
					 return new StyleProperty(StylePropertyId.TextGlowColor, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.TextGlowOffset:
					 return new StyleProperty(StylePropertyId.TextGlowOffset, 0f);
				case StylePropertyId.TextGlowInner:
					 return new StyleProperty(StylePropertyId.TextGlowInner, 0f);
				case StylePropertyId.TextGlowOuter:
					 return new StyleProperty(StylePropertyId.TextGlowOuter, 0f);
				case StylePropertyId.TextGlowPower:
					 return new StyleProperty(StylePropertyId.TextGlowPower, 0f);
				case StylePropertyId.TextUnderlayColor:
					 return new StyleProperty(StylePropertyId.TextUnderlayColor, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.TextUnderlayX:
					 return new StyleProperty(StylePropertyId.TextUnderlayX, 0f);
				case StylePropertyId.TextUnderlayY:
					 return new StyleProperty(StylePropertyId.TextUnderlayY, 0f);
				case StylePropertyId.TextUnderlayDilate:
					 return new StyleProperty(StylePropertyId.TextUnderlayDilate, 0f);
				case StylePropertyId.TextUnderlaySoftness:
					 return new StyleProperty(StylePropertyId.TextUnderlaySoftness, 0f);
				case StylePropertyId.TextFaceDilate:
					 return new StyleProperty(StylePropertyId.TextFaceDilate, 0f);
				case StylePropertyId.TextUnderlayType:
					 return new StyleProperty(StylePropertyId.TextUnderlayType, (int)UIForia.Rendering.UnderlayType.Unset);
				case StylePropertyId.TextTransform:
					 return new StyleProperty(StylePropertyId.TextTransform, (int)UIForia.Text.TextTransform.None);
				case StylePropertyId.TextWhitespaceMode:
					 return new StyleProperty(StylePropertyId.TextWhitespaceMode, (int)UIForia.Text.WhitespaceMode.CollapseWhitespaceAndTrim);
				case StylePropertyId.TransformPositionX:
					 return new StyleProperty(StylePropertyId.TransformPositionX, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.TransformPositionY:
					 return new StyleProperty(StylePropertyId.TransformPositionY, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.TransformPivotX:
					 return new StyleProperty(StylePropertyId.TransformPivotX, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.TransformPivotY:
					 return new StyleProperty(StylePropertyId.TransformPivotY, new UIFixedLength(0f, UIFixedUnit.Pixel));
				case StylePropertyId.TransformScaleX:
					 return new StyleProperty(StylePropertyId.TransformScaleX, 1f);
				case StylePropertyId.TransformScaleY:
					 return new StyleProperty(StylePropertyId.TransformScaleY, 1f);
				case StylePropertyId.TransformRotation:
					 return new StyleProperty(StylePropertyId.TransformRotation, 0f);
				case StylePropertyId.TransformBehaviorX:
					 return new StyleProperty(StylePropertyId.TransformBehaviorX, (int)UIForia.Rendering.TransformBehavior.LayoutOffset);
				case StylePropertyId.TransformBehaviorY:
					 return new StyleProperty(StylePropertyId.TransformBehaviorY, (int)UIForia.Rendering.TransformBehavior.LayoutOffset);
				case StylePropertyId.LayoutType:
					 return new StyleProperty(StylePropertyId.LayoutType, (int)UIForia.Layout.LayoutType.Flex);
				case StylePropertyId.LayoutBehavior:
					 return new StyleProperty(StylePropertyId.LayoutBehavior, (int)UIForia.Layout.LayoutBehavior.Normal);
				case StylePropertyId.ZIndex:
					 return new StyleProperty(StylePropertyId.ZIndex, 0);
				case StylePropertyId.RenderLayerOffset:
					 return new StyleProperty(StylePropertyId.RenderLayerOffset, 0);
				case StylePropertyId.RenderLayer:
					 return new StyleProperty(StylePropertyId.RenderLayer, (int)UIForia.Rendering.RenderLayer.Default);
				case StylePropertyId.Layer:
					 return new StyleProperty(StylePropertyId.Layer, 0);
				case StylePropertyId.ShadowColor:
					 return new StyleProperty(StylePropertyId.ShadowColor, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.ShadowTint:
					 return new StyleProperty(StylePropertyId.ShadowTint, new Color(-1f, -1f, -1f, -1f));
				case StylePropertyId.ShadowOffsetX:
					 return new StyleProperty(StylePropertyId.ShadowOffsetX, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.ShadowOffsetY:
					 return new StyleProperty(StylePropertyId.ShadowOffsetY, new OffsetMeasurement(0f, OffsetMeasurementUnit.Pixel));
				case StylePropertyId.ShadowSizeX:
					 return new StyleProperty(StylePropertyId.ShadowSizeX, 0f);
				case StylePropertyId.ShadowSizeY:
					 return new StyleProperty(StylePropertyId.ShadowSizeY, 0f);
				case StylePropertyId.ShadowIntensity:
					 return new StyleProperty(StylePropertyId.ShadowIntensity, 10f);
				case StylePropertyId.ShadowOpacity:
					 return new StyleProperty(StylePropertyId.ShadowOpacity, 1f);
				default: throw new System.ArgumentOutOfRangeException(nameof(propertyId), propertyId, null);
				}
} 
}
}