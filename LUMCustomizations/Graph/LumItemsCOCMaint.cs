using System;
using LumCustomizations.DAC;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using System.Linq;
using PX.Data.BQL;
using PX.SM;
using HtmlAgilityPack;

namespace LumCustomizations.Graph
{
    public class LumItemsCOCMaint : PXGraph<LumItemsCOCMaint>
    {
        #region Buttons definition		

        public PXSave<LumItemsCOC> Save;

        public PXCancel<LumItemsCOC> Cancel;

        #endregion

        #region Data View

        public SelectFrom<LumItemsCOC>.View _viewItemsCOC;

        public SelectFrom<LumItemsCOC>
                .Where<LumItemsCOC.inventoryID.IsEqual<LumItemsCOC.inventoryID.FromCurrent>>
                .View _viewLine;

        #endregion

        #region Event

        public LumItemsCOCMaint()
        {
            var _dataAttribute = SelectFrom<CSAttributeDetail>
                        .Where<CSAttributeDetail.attributeID.IsEqual<@P.AsString>>
                        .View.Select(this, "ENDC");
            PXStringListAttribute.SetList<LumItemsCOC.endCustomer>(base.Caches[typeof(LumItemsCOC)], null, _dataAttribute.FirstTableItems.Select(x => x.ValueID).ToArray(), _dataAttribute.FirstTableItems.Select(x => x.Description).ToArray());

        }

        public override void Persist()
        {
            // Current Data
            var _currData = (LumItemsCOC)_viewLine.Cache.Current;
            // Create Upload Image Graph
            UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
            // Html to Image and Upload to Server
            Action<string, string> htmlToImage = (_html, _fileName) =>
            {
                HtmlDocument _doc = new HtmlDocument();
                _doc.LoadHtml(_html);
                if (string.IsNullOrEmpty(_doc.DocumentNode.InnerText))
                    return;
                var htmlNode = _doc.DocumentNode.SelectSingleNode("//html");
                HtmlNode newNode = HtmlNode.CreateNode(@"<meta http-equiv=""content-type"" content=""text/html; charset=utf-8"" />");
                htmlNode.InsertBefore(newNode, htmlNode.ChildNodes[0]);
                var htmlToImageConv = new NReco.ImageGenerator.HtmlToImageConverter();
                var jpegBytes = htmlToImageConv.GenerateImage(_doc.DocumentNode.OuterHtml, NReco.ImageGenerator.ImageFormat.Jpeg);

                FileInfo file = new FileInfo($"{_fileName}.jpeg", null, jpegBytes);
                upload.SaveFile(file, FileExistsAction.CreateVersion);

                //To Attach file to Entity
                PXNoteAttribute.SetFileNotes(_viewLine.Cache, _viewLine.Cache.Current, file.UID.Value);
            };
            // Process All Tab
           

            #region MaterialProductDesc
           
            if (!CheckInnerTextIsEmpty(_currData.MaterialProductDesc))
                htmlToImage(_currData.MaterialProductDesc, $"COC_{_currData.InventoryID}_MaterialProductDesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.materialProductDesc>(_viewLine.Cache.Current, null); 
            #endregion

            #region MaterialProductDesc2
            if (!CheckInnerTextIsEmpty(_currData.MaterialProductDesc2))
                htmlToImage(_currData.MaterialProductDesc2, $"COC_{_currData.InventoryID}_MaterialProductDesc2");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.materialProductDesc2>(_viewLine.Cache.Current, null);
            #endregion;

            #region COCProductDesc
            if (!CheckInnerTextIsEmpty(_currData.COCProductDesc))
                htmlToImage(_currData.COCProductDesc, $"COC_{_currData.InventoryID}_COCProductDesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.cOCProductDesc>(_viewLine.Cache.Current, null);
            #endregion

            #region TESTProductDesc

            if (!CheckInnerTextIsEmpty(_currData.TESTProductDesc))
                htmlToImage(_currData.TESTProductDesc, $"COC_{_currData.InventoryID}_TESTProductDesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.tESTProductDesc>(_viewLine.Cache.Current, null);
            #endregion

            #region REROHSProductDesc

            if (!CheckInnerTextIsEmpty(_currData.REROHSProductDesc))
                htmlToImage(_currData.REROHSProductDesc, $"COC_{_currData.InventoryID}_REROHSProductDesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.rEROHSProductDesc>(_viewLine.Cache.Current, null);
            #endregion

            #region REACHProductDesc
            if (!CheckInnerTextIsEmpty(_currData.REACHProductDesc))
                htmlToImage(_currData.REACHProductDesc, $"COC_{_currData.InventoryID}_REACHProductDesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.rEACHProductDesc>(_viewLine.Cache.Current, null);
            #endregion

            #region REACHProductDesc2
            if (!CheckInnerTextIsEmpty(_currData.REACHProductDesc2))
                htmlToImage(_currData.REACHProductDesc2, $"COC_{_currData.InventoryID}_REACHProductDesc2");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.rEACHProductDesc2>(_viewLine.Cache.Current, null);

            #endregion

            #region Compliantproductdesc
            if (!CheckInnerTextIsEmpty(_currData.Compliantproductdesc))
                htmlToImage(_currData.Compliantproductdesc, $"COC_{_currData.InventoryID}_Compliantproductdesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.compliantproductdesc>(_viewLine.Cache.Current, null);
            #endregion

            #region QCProductDesc
            if (!CheckInnerTextIsEmpty(_currData.QCProductDesc))
                htmlToImage(_currData.QCProductDesc, $"COC_{_currData.InventoryID}_QCProductDesc");
            else
                _viewLine.Cache.SetValue<LumItemsCOC.qCProductDesc>(_viewLine.Cache.Current, null); 
            #endregion

            base.Persist();
        }

        #endregion

        public bool CheckInnerTextIsEmpty(string _html)
        {
            if (string.IsNullOrEmpty(_html))
                return true;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(_html);
            var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
            return bodyNode == null ? true : string.IsNullOrEmpty(bodyNode.InnerText) 
                                    ? true : false;
        }

    }
}