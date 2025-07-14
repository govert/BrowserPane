using System.Runtime.InteropServices;
using ExcelDna.Integration.CustomUI;

namespace BrowserPane
{
    [ComVisible(true)]
    public class Ribbon : ExcelRibbon
    {
        public override string GetCustomUI(string RibbonID)
        {
            return
@"<customUI xmlns='http://schemas.microsoft.com/office/2006/01/customui' loadImage='LoadImage'>
    <ribbon>
    <tabs>
        <tab id='Tab1' label='Browser Pane'>
        <group id='Group1' label='CTP Control'>
            <button id='Button1' label='Show Browser' size='large' onAction='OnShowCTP' />
            <button id='Button2' label='Remove Browser' size='large' onAction='OnDeleteCTP' />
        </group >
        </tab>
    </tabs>
    </ribbon>
</customUI>
";
        }

        public void OnShowCTP(IRibbonControl control)
        {
            CTPManager.ShowCTP();
        }


        public void OnDeleteCTP(IRibbonControl control)
        {
            CTPManager.DeleteCTP();
        }
    }
}