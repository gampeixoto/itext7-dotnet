using System.Collections.Generic;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Action {
    public class PdfActionOcgStateTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void PdfActionOcgStateUsageTest() {
            PdfName stateName = PdfName.ON;
            PdfDictionary ocgDict1 = new PdfDictionary();
            ocgDict1.Put(PdfName.Type, PdfName.OCG);
            ocgDict1.Put(PdfName.Name, new PdfName("ocg1"));
            PdfDictionary ocgDict2 = new PdfDictionary();
            ocgDict2.Put(PdfName.Type, PdfName.OCG);
            ocgDict2.Put(PdfName.Name, new PdfName("ocg2"));
            IList<PdfDictionary> dicts = new List<PdfDictionary>();
            dicts.Add(ocgDict1);
            dicts.Add(ocgDict2);
            PdfActionOcgState ocgState = new PdfActionOcgState(stateName, dicts);
            NUnit.Framework.Assert.AreEqual(stateName, ocgState.GetState());
            NUnit.Framework.Assert.AreEqual(dicts, ocgState.GetOcgs());
            IList<PdfObject> states = ocgState.GetObjectList();
            NUnit.Framework.Assert.AreEqual(3, states.Count);
            NUnit.Framework.Assert.AreEqual(stateName, states[0]);
            NUnit.Framework.Assert.AreEqual(ocgDict1, states[1]);
            NUnit.Framework.Assert.AreEqual(ocgDict2, states[2]);
        }
    }
}