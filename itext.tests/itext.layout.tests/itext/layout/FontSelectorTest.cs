/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout.Element;
using iText.Layout.Font;
using iText.Layout.Properties;
using iText.Test;

namespace iText.Layout {
    public class FontSelectorTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/FontSelectorTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/layout/FontSelectorTest/";

        public static readonly String fontsFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/layout/fonts/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CyrillicAndLatinGroup() {
            String fileName = "cyrillicAndLatinGroup";
            String outFileName = destinationFolder + "cyrillicAndLatinGroup.pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "NotoSans-Regular.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "FreeSans.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "Puritan2.otf", PdfEncodings.IDENTITY_H
                , "Puritan42"));
            String s = "Hello world! Здравствуй мир! Hello world! Здравствуй мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetProperty(Property.FONT, "Puritan42");
            Text text = new Text(s).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CyrillicAndLatinGroup2() {
            // TODO DEVSIX-2120 The font-family name of Puritan2.otf is 'Puritan 2.0' but this name doesn't match font-family name pattern
            String fileName = "cyrillicAndLatinGroup2";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "Puritan2.otf"));
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "NotoSans-Regular.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "FreeSans.ttf"));
            String s = "Hello world! Здравствуй мир! Hello world! Здравствуй мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetFont("'Puritan', \"FreeSans\"");
            // TODO DEVSIX-2120 font-family is Puritan 2.0 here, however it doesn't match font-family pattern
            Text text = new Text(s).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void LatinAndNotdefGroup() {
            String fileName = "latinAndNotdefGroup";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "Puritan2.otf"));
            String s = "Hello мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetFont("Puritan");
            Text text = new Text(s).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomFontWeight() {
            String fileName = "customFontWeight";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            sel.GetFontSet().AddFont(StandardFonts.HELVETICA);
            sel.GetFontSet().AddFont(StandardFonts.HELVETICA_BOLD);
            sel.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN);
            // The provided alias is incorrect. It'll be used as a font's family, but since the name is invalid, the font shouldn't be selected
            sel.GetFontSet().AddFont(StandardFonts.TIMES_BOLD, null, "Times-Roman Bold");
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            Div div = new Div().SetFont(StandardFonts.TIMES_ROMAN);
            Paragraph paragraph = new Paragraph("Times Roman Bold text");
            paragraph.SetProperty(Property.FONT_WEIGHT, "bold");
            div.Add(paragraph);
            doc.Add(div);
            doc.Add(new Paragraph("UPD: The paragraph above should be written in Helvetica-Bold. The provided alias for Times-Bold was incorrect. It was used as a font's family, but since the name is invalid, the font wasn't selected."
                ));
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomFontWeight2() {
            String fileName = "customFontWeight2";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            sel.GetFontSet().AddFont(StandardFonts.HELVETICA);
            sel.GetFontSet().AddFont(StandardFonts.HELVETICA_BOLD);
            sel.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN);
            sel.GetFontSet().AddFont(StandardFonts.TIMES_BOLD);
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            Div div = new Div().SetFont(StandardFontFamilies.TIMES);
            // TODO DEVSIX-2136 Update of necessary
            Paragraph paragraph = new Paragraph("Times Roman Bold text");
            paragraph.SetProperty(Property.FONT_WEIGHT, "bold");
            div.Add(paragraph);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CustomFontWeight3() {
            String fileName = "customFontWeight3";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            sel.GetFontSet().AddFont(StandardFonts.HELVETICA);
            sel.GetFontSet().AddFont(StandardFonts.HELVETICA_BOLD);
            sel.GetFontSet().AddFont(StandardFonts.TIMES_ROMAN);
            // correct alias
            sel.GetFontSet().AddFont(StandardFonts.TIMES_BOLD);
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            Div div = new Div().SetFont(StandardFontFamilies.TIMES);
            // TODO DEVSIX-2136 Update of necessary
            Paragraph paragraph = new Paragraph("Times Roman Bold text");
            paragraph.SetProperty(Property.FONT_WEIGHT, "bold");
            div.Add(paragraph);
            doc.Add(div);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void StandardPdfFonts() {
            String fileName = "standardPdfFonts";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            sel.AddStandardPdfFonts();
            String s = "Hello world!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            Paragraph paragraph = new Paragraph(s);
            paragraph.SetFont("Courier");
            doc.Add(paragraph);
            paragraph = new Paragraph(s);
            paragraph.SetProperty(Property.FONT, "Times");
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SearchNames() {
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "NotoSans-Regular.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.AddFont(fontsFolder + "FreeSans.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "Puritan2.otf", PdfEncodings.IDENTITY_H
                , "Puritan42"));
            ICollection<FontInfo> fonts = sel.GetFontSet().Get("puritan2");
            NUnit.Framework.Assert.IsTrue(fonts.Count != 0, "Puritan not found!");
            FontInfo puritan = GetFirst(fonts);
            NUnit.Framework.Assert.IsFalse(sel.GetFontSet().AddFont(puritan, "Puritan42"), "Replace existed font");
            NUnit.Framework.Assert.IsFalse(sel.GetFontSet().AddFont(puritan), "Replace existed font");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("NotoSans"), "NotoSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("Noto Sans"), "NotoSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("FreeSans"), "FreeSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("Free Sans"), "FreeSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("puritan 2.0 regular"), "Puritan 2.0 not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("puritan2"), "Puritan 2.0 not found!");
            NUnit.Framework.Assert.IsFalse(sel.GetFontSet().Contains("puritan42"), "Puritan42 found!");
            NUnit.Framework.Assert.AreEqual(puritan, GetFirst(sel.GetFontSet().Get("puritan 2.0 regular")), "Puritan 2.0 not found!"
                );
            NUnit.Framework.Assert.AreEqual(puritan, GetFirst(sel.GetFontSet().Get("puritan2")), "Puritan 2.0 not found!"
                );
            NUnit.Framework.Assert.IsTrue(GetFirst(sel.GetFontSet().Get("puritan42")) == null, "Puritan42 found!");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SearchNames2() {
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "FreeSans.ttf"));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "Puritan2.otf", PdfEncodings.IDENTITY_H
                , "Puritan42"));
            ICollection<FontInfo> fonts = sel.GetFontSet().Get("puritan2");
            NUnit.Framework.Assert.IsTrue(fonts.Count != 0, "Puritan not found!");
            FontInfo puritan = GetFirst(fonts);
            fonts = sel.GetFontSet().Get("NotoSans");
            NUnit.Framework.Assert.IsTrue(fonts.Count != 0, "NotoSans not found!");
            FontInfo notoSans = GetFirst(fonts);
            fonts = sel.GetFontSet().Get("FreeSans");
            NUnit.Framework.Assert.IsTrue(fonts.Count != 0, "FreeSans not found!");
            FontInfo freeSans = GetFirst(fonts);
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("NotoSans"), "NotoSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("Noto Sans"), "NotoSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("FreeSans"), "FreeSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("Free Sans"), "FreeSans not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("puritan 2.0 regular"), "Puritan 2.0 not found!");
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Contains("puritan2"), "Puritan 2.0 not found!");
            NUnit.Framework.Assert.IsFalse(sel.GetFontSet().Contains("puritan42"), "Puritan42 found!");
            NUnit.Framework.Assert.AreEqual(notoSans, GetFirst(sel.GetFontSet().Get("NotoSans")), "NotoSans not found!"
                );
            NUnit.Framework.Assert.AreEqual(notoSans, GetFirst(sel.GetFontSet().Get("Noto Sans")), "NotoSans not found!"
                );
            NUnit.Framework.Assert.AreEqual(freeSans, GetFirst(sel.GetFontSet().Get("FreeSans")), "FreeSans not found!"
                );
            NUnit.Framework.Assert.AreEqual(freeSans, GetFirst(sel.GetFontSet().Get("Free Sans")), "FreeSans not found!"
                );
            NUnit.Framework.Assert.AreEqual(puritan, GetFirst(sel.GetFontSet().Get("puritan 2.0 regular")), "Puritan 2.0 not found!"
                );
            NUnit.Framework.Assert.AreEqual(puritan, GetFirst(sel.GetFontSet().Get("puritan2")), "Puritan 2.0 not found!"
                );
            NUnit.Framework.Assert.IsTrue(GetFirst(sel.GetFontSet().Get("puritan42")) == null, "Puritan42 found!");
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CyrillicAndLatinWithUnicodeRange() {
            String fileName = "cyrillicAndLatinWithUnicodeRange";
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", null, "FontAlias"
                , new RangeBuilder(0, 255).Create()));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "FreeSans.ttf", null, "FontAlias", new 
                RangeBuilder(1024, 1279).Create()));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Size() == 2);
            String s = "Hello world! Здравствуй мир! Hello world! Здравствуй мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetProperty(Property.FONT, "FontAlias");
            Text text = new Text(s).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void DuplicateFontWithUnicodeRange() {
            String fileName = "duplicateFontWithUnicodeRange";
            //In the result pdf will be two equal fonts but with different subsets
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", null, "FontAlias"
                , new RangeBuilder(0, 255).Create()));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", null, "FontAlias"
                , new RangeBuilder(1024, 1279).Create()));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Size() == 2);
            String s = "Hello world! Здравствуй мир! Hello world! Здравствуй мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetProperty(Property.FONT, "FontAlias");
            Text text = new Text(s).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void SingleFontWithUnicodeRange() {
            String fileName = "singleFontWithUnicodeRange";
            //In the result pdf will be two equal fonts but with different subsets
            String outFileName = destinationFolder + fileName + ".pdf";
            String cmpFileName = sourceFolder + "cmp_" + fileName + ".pdf";
            FontProvider sel = new FontProvider();
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", null, "FontAlias"
                ));
            NUnit.Framework.Assert.IsFalse(sel.GetFontSet().AddFont(fontsFolder + "NotoSans-Regular.ttf", null, "FontAlias"
                ));
            NUnit.Framework.Assert.IsTrue(sel.GetFontSet().Size() == 1);
            String s = "Hello world! Здравствуй мир! Hello world! Здравствуй мир!";
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(new FileStream(outFileName, FileMode.Create)));
            Document doc = new Document(pdfDoc);
            doc.SetFontProvider(sel);
            doc.SetProperty(Property.FONT, "FontAlias");
            Text text = new Text(s).SetBackgroundColor(ColorConstants.LIGHT_GRAY);
            Paragraph paragraph = new Paragraph(text);
            doc.Add(paragraph);
            doc.Close();
            NUnit.Framework.Assert.IsNull(new CompareTool().CompareByContent(outFileName, cmpFileName, destinationFolder
                , "diff" + fileName));
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontSetTimesTest01() {
            CheckSelector(GetStandardFontSet().GetFonts(), "Times", "Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic"
                );
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontSetHelveticaTest01() {
            CheckSelector(GetStandardFontSet().GetFonts(), "Helvetica", "Helvetica", "Helvetica-Bold", "Helvetica-Oblique"
                , "Helvetica-BoldOblique");
        }

        [NUnit.Framework.Test]
        public virtual void StandardFontSetCourierTest01() {
            CheckSelector(GetStandardFontSet().GetFonts(), "Courier", "Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OpenSansFontSetIncorrectNameTest01() {
            // TODO DEVSIX-2120 Currently both light and regular fonts have the same score so that light is picked up lexicographically. After the changes are implemented the correct one (regular) font shall be selected and the expected constants should be updated
            // TODO Default font shall be specified.
            FontSet set = GetOpenSansFontSet();
            AddTimesFonts(set);
            CheckSelector(set.GetFonts(), "OpenSans", "OpenSans-Light", "OpenSans-Bold", "OpenSans-LightItalic", "OpenSans-BoldItalic"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OpenSansFontSetRegularTest01() {
            // TODO DEVSIX-2120 Currently both light and regular fonts have the same score so that light is picked up lexicographically. After the changes are implemented the correct one (regular) font shall be selected and the expected constants should be updated
            // TODO Default font shall be specified.
            FontSet set = GetOpenSansFontSet();
            AddTimesFonts(set);
            CheckSelector(set.GetFonts(), "Open Sans", "OpenSans-Light", "OpenSans-Bold", "OpenSans-LightItalic", "OpenSans-BoldItalic"
                );
        }

        [NUnit.Framework.Test]
        public virtual void OpenSansFontSetLightTest01() {
            // TODO DEVSIX-2127 After DEVSIX-2120 the font should be selected correctly, but the text will still need to be bolded via emulation
            // TODO DEVSIX-2120 Light subfamily is not processed
            // TODO Default font shall be specified.
            FontSet set = GetOpenSansFontSet();
            AddTimesFonts(set);
            CheckSelector(set.GetFonts(), "Open Sans Light", "OpenSans-Light", "OpenSans-Bold", "OpenSans-LightItalic"
                , "OpenSans-BoldItalic");
        }

        [NUnit.Framework.Test]
        [NUnit.Framework.Ignore("DEVSIX-2120: we cannot set a wrong expected string for normal font characteristics because in different contexts iText selects different fonts"
            )]
        public virtual void OpenSansFontSetExtraBoldTest01() {
            // TODO DEVSIX-2120 ExtraBold subfamily is not processed
            // TODO DEVSIX-2135 if FontCharacteristics instance is not modified, font-family is parsed and 'bold' substring is considered as a reason to set bold flag in FontCharacteristics instance. That should be reviewed.
            FontSet set = GetOpenSansFontSet();
            AddTimesFonts(set);
            CheckSelector(set.GetFonts(), "Open Sans ExtraBold", "Times-Bold", "Times-Bold", "Times-BoldItalic", "Times-BoldItalic"
                );
        }

        private void CheckSelector(ICollection<FontInfo> fontInfoCollection, String fontFamily, String expectedNormal
            , String expectedBold, String expectedItalic, String expectedBoldItalic) {
            IList<String> fontFamilies = new List<String>();
            fontFamilies.Add(fontFamily);
            // Normal
            FontCharacteristics fc = new FontCharacteristics();
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedNormal);
            fc = new FontCharacteristics();
            fc.SetFontWeight((short)300);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedNormal);
            fc = new FontCharacteristics();
            fc.SetFontWeight((short)100);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedNormal);
            fc = new FontCharacteristics();
            fc.SetFontWeight("normal");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedNormal);
            fc = new FontCharacteristics();
            fc.SetFontStyle("normal");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedNormal);
            // Bold
            fc = new FontCharacteristics();
            fc.SetBoldFlag(true);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBold);
            fc = new FontCharacteristics();
            fc.SetFontWeight("bold");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBold);
            fc = new FontCharacteristics();
            fc.SetFontWeight((short)700);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBold);
            fc = new FontCharacteristics();
            fc.SetFontWeight((short)800);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBold);
            // Italic
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            fc.SetFontWeight("normal");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            fc.SetFontWeight((short)300);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            fc.SetFontWeight((short)500);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("oblique");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedItalic);
            // BoldItalic
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            fc.SetFontWeight("bold");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBoldItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("oblique");
            fc.SetFontWeight("bold");
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBoldItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            fc.SetFontWeight((short)700);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBoldItalic);
            fc = new FontCharacteristics();
            fc.SetFontStyle("italic");
            fc.SetFontWeight((short)800);
            AssertSelectedFont(fontInfoCollection, fontFamilies, fc, expectedBoldItalic);
        }

        private void AssertSelectedFont(ICollection<FontInfo> fontInfoCollection, IList<String> fontFamilies, FontCharacteristics
             fc, String expectedFontName) {
            NUnit.Framework.Assert.AreEqual(expectedFontName, new FontSelector(fontInfoCollection, fontFamilies, fc).BestMatch
                ().GetDescriptor().GetFontName());
        }

        private static FontSet GetStandardFontSet() {
            FontSet set = new FontSet();
            set.AddFont(StandardFonts.COURIER);
            set.AddFont(StandardFonts.COURIER_BOLD);
            set.AddFont(StandardFonts.COURIER_BOLDOBLIQUE);
            set.AddFont(StandardFonts.COURIER_OBLIQUE);
            set.AddFont(StandardFonts.HELVETICA);
            set.AddFont(StandardFonts.HELVETICA_BOLD);
            set.AddFont(StandardFonts.HELVETICA_BOLDOBLIQUE);
            set.AddFont(StandardFonts.HELVETICA_OBLIQUE);
            set.AddFont(StandardFonts.SYMBOL);
            set.AddFont(StandardFonts.ZAPFDINGBATS);
            AddTimesFonts(set);
            return set;
        }

        private static FontSet GetOpenSansFontSet() {
            String openSansFolder = "Open_Sans/";
            FontSet set = new FontSet();
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-Bold.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-BoldItalic.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-ExtraBold.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-ExtraBoldItalic.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-Light.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-LightItalic.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-Regular.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-Italic.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-SemiBold.ttf");
            set.AddFont(fontsFolder + openSansFolder + "OpenSans-SemiBoldItalic.ttf");
            return set;
        }

        private static FontSet AddTimesFonts(FontSet set) {
            set.AddFont(StandardFonts.TIMES_ROMAN);
            set.AddFont(StandardFonts.TIMES_BOLD);
            set.AddFont(StandardFonts.TIMES_BOLDITALIC);
            set.AddFont(StandardFonts.TIMES_ITALIC);
            return set;
        }

        private static FontInfo GetFirst(ICollection<FontInfo> fonts) {
            if (fonts.Count != 1) {
                return null;
            }
            //noinspection LoopStatementThatDoesntLoop
            foreach (FontInfo fi in fonts) {
                return fi;
            }
            return null;
        }
    }
}
