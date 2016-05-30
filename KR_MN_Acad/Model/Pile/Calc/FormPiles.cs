using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using AcadLib;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace KR_MN_Acad.Model.Pile.Calc
{
    public partial class FormPiles : Form
    {
        private Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

        public FormPiles(List<HightMark.HightMarkRow> rowsHM, List<Spec.SpecRow> rowsCalc)
        {
            InitializeComponent();

            try
            {
                UpdateData(rowsHM, rowsCalc);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "FormPiles UpdateData()");
            }
        }

        public void ButtonDialogVisible(bool visible)
        {
            buttonOk.Visible = visible;
            buttonCancel.Visible = visible;
        }

        private void UpdateData(List<HightMark.HightMarkRow> rowsHM, List<Spec.SpecRow> rowsCalc)
        {
            ImageList im = new ImageList();            
            treeViewPiles.ImageList = im;
            treeViewPiles.ImageIndex = 100;
            treeViewPiles.SelectedImageIndex = 100;

            var views = rowsHM.GroupBy(g => g.View);
            foreach (var item in views)
            {
                using (var btr = item.First().IdBtr.Open( Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead) as BlockTableRecord)
                {
                    var icon = AcadLib.Blocks.Visual.BlockPreviewHelper.GetPreviewIcon(btr);
                    im.Images.Add(item.Key, icon);
                }
            }

            TreeNode rootHM = new TreeNode("Отметки свай");            
            treeViewPiles.Nodes.Add(rootHM);
            TreeNode rootCalc = new TreeNode("Спецификация свай");
            treeViewPiles.Nodes.Add(rootCalc);            

            rootHM.Expand();
            rootCalc.Expand();            

            foreach (var row in rowsHM)
            {   
                TreeNode nodeRow = new TreeNode(row.Info);
                nodeRow.ImageKey = row.View;
                nodeRow.SelectedImageKey = row.View;
                rootHM.Nodes.Add(nodeRow);
                foreach (var pile in row.Piles)
                {
                    TreeNode nodePile = new TreeNode(pile.Pos.ToString());                    
                    nodePile.Tag = pile;
                    nodeRow.Nodes.Add(nodePile);
                }
            }

            foreach (var row in rowsCalc)
            {
                TreeNode nodeRow = new TreeNode(row.Info);
                nodeRow.ImageKey = row.View;
                nodeRow.SelectedImageKey = row.View;
                rootCalc.Nodes.Add(nodeRow);
                foreach (var pile in row.Piles)
                {
                    TreeNode nodePile = new TreeNode(pile.Pos.ToString());                    
                    nodePile.Tag = pile;
                    nodeRow.Nodes.Add(nodePile);
                }
            }
        }

        private void Show(object sender, EventArgs e)
        {
            var pile = treeViewPiles.SelectedNode?.Tag as Pile;
            if (pile != null)
            {
                Document curDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                if (ed.Document != curDoc)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog($"Должен быть активен документ {ed.Document.Name}");
                    return;
                }
                pile.Show(ed);
            }
        }

        private void treeViewPiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var pile = treeViewPiles.SelectedNode?.Tag as Pile;
            buttonShow.Enabled = (pile != null);
        }
    }
}
