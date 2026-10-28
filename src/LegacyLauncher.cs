using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
class LegacyLauncher {
 [STAThread] static void Main(){
  string folder=AppDomain.CurrentDomain.BaseDirectory;
  string target=Path.Combine(folder,"FlowDesk.exe");
  if(!File.Exists(target))target=Path.GetFullPath(Path.Combine(folder,"..","FlowDesk.exe"));
  if(File.Exists(target))Process.Start(new ProcessStartInfo(target){UseShellExecute=true});
  else MessageBox.Show("Open FlowDesk.exe from the updated Flow Desk folder.","Flow Desk");
 }
}
