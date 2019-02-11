using System;
using System.Collections.Generic;
using iText.IO.Util;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Exceptions;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;text&gt; and &lt;tspan&gt; tag.
    /// </summary>
    public class TextSvgBranchRenderer : AbstractSvgNodeRenderer, ISvgTextNodeRenderer {
        /// <summary>Top level transformation to flip the y-axis results in the character glyphs being mirrored, this tf corrects for this behaviour
        ///     </summary>
        protected internal static readonly AffineTransform TEXTFLIP = new AffineTransform(1, 0, 0, -1, 0, 0);

        /// <summary>Placeholder default font-size until DEVSIX-2607 is resolved</summary>
        private const float DEFAULT_FONT_SIZE = 12f;

        private readonly IList<ISvgTextNodeRenderer> children = new List<ISvgTextNodeRenderer>();

        protected internal bool performRootTransformations;

        private PdfFont font;

        private float fontSize;

        private bool moveResolved;

        private float xMove;

        private float yMove;

        private bool posResolved;

        private float[] xPos;

        private float[] yPos;

        private bool whiteSpaceProcessed = false;

        public TextSvgBranchRenderer() {
            performRootTransformations = true;
            moveResolved = false;
            posResolved = false;
        }

        public override ISvgNodeRenderer CreateDeepCopy() {
            iText.Svg.Renderers.Impl.TextSvgBranchRenderer copy = new iText.Svg.Renderers.Impl.TextSvgBranchRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public void AddChild(ISvgTextNodeRenderer child) {
            // final method, in order to disallow adding null
            if (child != null) {
                children.Add(child);
            }
        }

        public IList<ISvgTextNodeRenderer> GetChildren() {
            // final method, in order to disallow modifying the List
            return JavaCollectionsUtil.UnmodifiableList(children);
        }

        public virtual float GetTextContentLength(float parentFontSize, PdfFont font) {
            return 0.0f;
        }

        //Branch renderers do not contain any text themselves and do not contribute to the text length
        public virtual float[] GetRelativeTranslation() {
            if (!moveResolved) {
                ResolveTextMove();
            }
            return new float[] { xMove, yMove };
        }

        public virtual bool ContainsRelativeMove() {
            if (!moveResolved) {
                ResolveTextMove();
            }
            bool isNullMove = CssUtils.CompareFloats(0f, xMove) && CssUtils.CompareFloats(0f, yMove);
            // comparision to 0
            return !isNullMove;
        }

        public virtual bool ContainsAbsolutePositionChange() {
            if (!posResolved) {
                ResolveTextPosition();
            }
            return (xPos != null && xPos.Length > 0) || (yPos != null && yPos.Length > 0);
        }

        public virtual float[][] GetAbsolutePositionChanges() {
            if (!posResolved) {
                ResolveTextPosition();
            }
            return new float[][] { xPos, yPos };
        }

        public virtual void MarkWhiteSpaceProcessed() {
            whiteSpaceProcessed = true;
        }

        /// <summary>
        /// Method that will set properties to be inherited by this branch renderer's
        /// children and will iterate over all children in order to draw them.
        /// </summary>
        /// <param name="context">
        /// the object that knows the place to draw this element and
        /// maintains its state
        /// </param>
        protected internal override void DoDraw(SvgDrawContext context) {
            if (GetChildren().Count > 0) {
                // if branch has no children, don't do anything
                PdfCanvas currentCanvas = context.GetCurrentCanvas();
                if (performRootTransformations) {
                    currentCanvas.BeginText();
                    //Current transformation matrix results in the character glyphs being mirrored, correct with inverse tf
                    AffineTransform rootTf;
                    if (this.ContainsAbsolutePositionChange()) {
                        rootTf = GetTextTransform(this.GetAbsolutePositionChanges(), context);
                    }
                    else {
                        rootTf = new AffineTransform(TEXTFLIP);
                    }
                    currentCanvas.SetTextMatrix(rootTf);
                    //Reset context of text move
                    context.ResetTextMove();
                    //Apply relative move
                    if (this.ContainsRelativeMove()) {
                        float[] rootMove = this.GetRelativeTranslation();
                        context.AddTextMove(rootMove[0], -rootMove[1]);
                    }
                    //-y to account for the text-matrix transform we do in the text root to account for the coordinates
                    //handle white-spaces
                    if (!whiteSpaceProcessed) {
                        SvgTextUtil.ProcessWhiteSpace(this, true);
                    }
                }
                ApplyTextRenderingMode(currentCanvas);
                if (this.attributesAndStyles != null) {
                    ResolveFontSize();
                    ResolveFont(context);
                    currentCanvas.SetFontAndSize(font, fontSize);
                    foreach (ISvgTextNodeRenderer c in children) {
                        if (c.ContainsAbsolutePositionChange()) {
                            //TODO(DEVSIX-2507) support rotate and other attributes
                            float[][] absolutePositions = c.GetAbsolutePositionChanges();
                            AffineTransform newTransform = GetTextTransform(absolutePositions, context);
                            //overwrite the last transformation stored in the context
                            context.SetLastTextTransform(newTransform);
                            //Apply transformation
                            currentCanvas.SetTextMatrix(newTransform);
                            //Absolute position changes requires resetting the current text move in the context
                            context.ResetTextMove();
                        }
                        //Move needs to happen before the saving of the state in order for it to cascade beyond
                        if (c.ContainsRelativeMove()) {
                            float[] childMove = c.GetRelativeTranslation();
                            context.AddTextMove(childMove[0], -childMove[1]);
                        }
                        //-y to account for the text-matrix transform we do in the text root to account for the coordinates
                        currentCanvas.SaveState();
                        c.Draw(context);
                        float length = c.GetTextContentLength(fontSize, font);
                        context.AddTextMove(length, 0);
                        currentCanvas.RestoreState();
                        //Restore transformation matrix
                        if (!context.GetLastTextTransform().IsIdentity()) {
                            currentCanvas.SetTextMatrix(context.GetLastTextTransform());
                        }
                    }
                    if (performRootTransformations) {
                        currentCanvas.EndText();
                    }
                }
            }
        }

        private void ResolveTextMove() {
            if (this.attributesAndStyles != null) {
                String xRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.DX);
                String yRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.DY);
                IList<String> xValuesList = SvgCssUtils.SplitValueList(xRawValue);
                IList<String> yValuesList = SvgCssUtils.SplitValueList(yRawValue);
                xMove = 0f;
                yMove = 0f;
                if (!xValuesList.IsEmpty()) {
                    xMove = CssUtils.ParseAbsoluteLength(xValuesList[0]);
                }
                if (!yValuesList.IsEmpty()) {
                    yMove = CssUtils.ParseAbsoluteLength(yValuesList[0]);
                }
                moveResolved = true;
            }
        }

        private FontInfo ResolveFontName(String fontFamily, String fontWeight, String fontStyle, FontProvider provider
            , FontSet tempFonts) {
            bool isBold = fontWeight != null && fontWeight.EqualsIgnoreCase(SvgConstants.Attributes.BOLD);
            bool isItalic = fontStyle != null && fontStyle.EqualsIgnoreCase(SvgConstants.Attributes.ITALIC);
            FontCharacteristics fontCharacteristics = new FontCharacteristics();
            IList<String> stringArrayList = new List<String>();
            stringArrayList.Add(fontFamily);
            fontCharacteristics.SetBoldFlag(isBold);
            fontCharacteristics.SetItalicFlag(isItalic);
            return provider.GetFontSelector(stringArrayList, fontCharacteristics, tempFonts).BestMatch();
        }

        private void ResolveFont(SvgDrawContext context) {
            FontProvider provider = context.GetFontProvider();
            FontSet tempFonts = context.GetTempFonts();
            font = null;
            if (!provider.GetFontSet().IsEmpty() || (tempFonts != null && !tempFonts.IsEmpty())) {
                String fontFamily = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_FAMILY);
                String fontWeight = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_WEIGHT);
                String fontStyle = this.attributesAndStyles.Get(SvgConstants.Attributes.FONT_STYLE);
                fontFamily = fontFamily != null ? fontFamily.Trim() : "";
                FontInfo fontInfo = ResolveFontName(fontFamily, fontWeight, fontStyle, provider, tempFonts);
                font = provider.GetPdfFont(fontInfo, tempFonts);
            }
            if (font == null) {
                try {
                    // TODO (DEVSIX-2057)
                    // TODO each call of createFont() create a new instance of PdfFont.
                    // TODO FontProvider shall be used instead.
                    font = PdfFontFactory.CreateFont();
                }
                catch (System.IO.IOException e) {
                    throw new SvgProcessingException(SvgLogMessageConstant.FONT_NOT_FOUND, e);
                }
            }
        }

        private void ResolveFontSize() {
            //TODO (DEVSIX-2607) (re)move static variable
            fontSize = (float)SvgTextUtil.ResolveFontSize(this, DEFAULT_FONT_SIZE);
        }

        private void ResolveTextPosition() {
            if (this.attributesAndStyles != null) {
                String xRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.X);
                String yRawValue = this.attributesAndStyles.Get(SvgConstants.Attributes.Y);
                xPos = GetPositionsFromString(xRawValue);
                yPos = GetPositionsFromString(yRawValue);
                posResolved = true;
            }
        }

        private static float[] GetPositionsFromString(String rawValuesString) {
            float[] result = null;
            IList<String> valuesList = SvgCssUtils.SplitValueList(rawValuesString);
            if (!valuesList.IsEmpty()) {
                result = new float[valuesList.Count];
                for (int i = 0; i < valuesList.Count; i++) {
                    result[i] = CssUtils.ParseAbsoluteLength(valuesList[i]);
                }
            }
            return result;
        }

        private static AffineTransform GetTextTransform(float[][] absolutePositions, SvgDrawContext context) {
            AffineTransform tf = new AffineTransform();
            //If x is not specified, but y is, we need to correct for preceding text.
            if (absolutePositions[0] == null && absolutePositions[1] != null) {
                absolutePositions[0] = new float[] { context.GetTextMove()[0] };
            }
            //If y is not present, we can replace it with a neutral transformation (0.0f)
            if (absolutePositions[1] == null) {
                absolutePositions[1] = new float[] { 0.0f };
            }
            tf.Concatenate(TEXTFLIP);
            tf.Concatenate(AffineTransform.GetTranslateInstance(absolutePositions[0][0], -absolutePositions[1][0]));
            return tf;
        }

        private void ApplyTextRenderingMode(PdfCanvas currentCanvas) {
            //Fill only is the default for text operation in PDF
            if (doStroke && doFill) {
                currentCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL_STROKE);
            }
            else {
                //Default for SVG
                if (doStroke) {
                    currentCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.STROKE);
                }
                else {
                    currentCanvas.SetTextRenderingMode(PdfCanvasConstants.TextRenderingMode.FILL);
                }
            }
        }

        private void DeepCopyChildren(iText.Svg.Renderers.Impl.TextSvgBranchRenderer deepCopy) {
            foreach (ISvgTextNodeRenderer child in children) {
                ISvgTextNodeRenderer newChild = (ISvgTextNodeRenderer)child.CreateDeepCopy();
                child.SetParent(deepCopy);
                deepCopy.AddChild(newChild);
            }
        }
    }
}