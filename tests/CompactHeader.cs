using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FlowDesk;
class CompactHeader {
 [STAThread] static int Main(){
  Application.EnableVisualStyles();
  string previous=Store.Root;Store.Root=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"header-test-settings");
  try{
   FlowDesk.Program.Context=new FlowDesk.AppContext();
   var model=new WorkspaceModel{Name="Compact header check"};
   model.Panels[0].Tabs.Add(new TabModel{Name="First application window"});
   model.Panels[0].Tabs.Add(new TabModel{Name="Another application with a much longer title"});
   using(var form=new Workspace(model)){
    FlowDesk.Program.Context.Forms.Add(form);form.Show();Application.DoEvents();
    var tabs=form.Panes[0].Tabs;if(tabs.TabCount!=0)throw new Exception("Saved metadata created empty reconnect tabs");
    EmptyWorkspace empty=null;foreach(Control child in tabs.Parent.Controls)if(child is EmptyWorkspace)empty=(EmptyWorkspace)child;
    if(empty==null||!empty.Visible||tabs.Parent.Controls.GetChildIndex(empty)!=0)throw new Exception("Empty state covered by native white control");
    using(var image=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(image,form.ClientRectangle);Color background=image.GetPixel(form.Width/2,form.Height/3);if(background.R>80||background.G>80||background.B>80)throw new Exception("Empty background is not dark");image.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"empty-workspace.png"));}
    var mouseDown=typeof(Control).GetMethod("OnMouseDown",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
    mouseDown.Invoke(tabs.Header,new object[]{new MouseEventArgs(MouseButtons.Left,1,tabs.Header.Width-30,12,0)});
    if(!form.Capture)throw new Exception("Empty header mouse routing cannot start dragging");form.Capture=false;
    foreach(var saved in model.Panels[0].Tabs)tabs.TabPages.Add(new TabPage(saved.Name){Tag=saved});Application.DoEvents();
    if(tabs.Header.Height!=24||tabs.GetTabRect(0).Left!=30)throw new Exception("Header geometry");
    if(tabs.GetTabRect(1).Width>185)throw new Exception("Tab width not bounded");
    Point header=tabs.Header.PointToScreen(Point.Empty);
    for(int i=0;i<20;i++){
     tabs.SelectedIndex=i%2;Application.DoEvents();
     Point body=tabs.SelectedTab.PointToScreen(Point.Empty);
     if(Math.Abs(body.Y-(header.Y+tabs.Header.Height))>2)throw new Exception("Native tab strip not clipped: "+body.Y+" / "+header.Y);
    }
    var originalFont=tabs.Font;
    using(var changedFont=new Font("Segoe UI",18)){
     tabs.Font=changedFont;
     tabs.TabPages.Add(new TabPage("Added after layout"){Tag=new TabModel{Name="Added after layout"}});
     tabs.SelectedIndex=tabs.TabCount-1;Application.DoEvents();
     Point body=tabs.SelectedTab.PointToScreen(Point.Empty);
     if(Math.Abs(body.Y-(tabs.Header.PointToScreen(Point.Empty).Y+tabs.Header.Height))>2)throw new Exception("Hidden native header reappeared after runtime metrics changed");
     if(tabs.Region==null||tabs.Region.IsVisible(10,5))throw new Exception("Hidden header remains in native input region");
     var hit=new Rect{Left=10,Top=5};
     if(Native.AdjustTabRect(tabs.Handle,0x130d,IntPtr.Zero,ref hit)!=new IntPtr(-1))throw new Exception("Native hidden tab still responds to hit tests");
     tabs.Font=originalFont;Application.DoEvents();
    }
    using(var image=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(image,form.ClientRectangle);
     foreach(var point in new[]{new Point(1,form.Height/2),new Point(form.Width-2,form.Height/2),new Point(form.Width/2,1),new Point(form.Width/2,form.Height-2)}){Color color=image.GetPixel(point.X,point.Y);if(color.R>180&&color.G>180&&color.B>180)throw new Exception("Light workspace edge at "+point);}
     image.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"compact-header.png"));}
    if(form.FormBorderStyle!=FormBorderStyle.None)throw new Exception("Caption still visible");
    if(form.WindowHitTest(form.PointToScreen(new Point(1,1)))!=13)throw new Exception("Resize corner not reachable");
    if(form.WindowHitTest(tabs.Header.PointToScreen(new Point(tabs.Header.Width-20,12)))!=2)throw new Exception("Empty tab row cannot drag window");
    if(form.WindowHitTest(tabs.Header.PointToScreen(new Point(15,12)))!=1)throw new Exception("Menu incorrectly drags window");
    Rectangle normal=form.Bounds;form.ToggleMaximize();Application.DoEvents();
    if(form.Bounds!=Screen.FromHandle(form.Handle).WorkingArea)throw new Exception("Maximize does not respect taskbar");
    form.ToggleMaximize();Application.DoEvents();
    if(form.Bounds!=normal)throw new Exception("Maximize restore changed bounds");
    form.ToggleFullScreen();Application.DoEvents();
    if(form.Bounds!=Screen.FromHandle(form.Handle).Bounds)throw new Exception("Full screen bounds");
    if(form.Padding!=Padding.Empty)throw new Exception("Full screen still has host margins");
    Rectangle page=tabs.SelectedTab.RectangleToScreen(tabs.SelectedTab.ClientRectangle),monitor=Screen.FromHandle(form.Handle).Bounds;
    if(page.Left!=monitor.Left||page.Right!=monitor.Right)throw new Exception("Full screen content does not reach both screen edges");
    form.ToggleFullScreen();Application.DoEvents();
    if(form.Bounds!=normal)throw new Exception("Full screen restore changed bounds");
    if(form.Padding!=new Padding(3))throw new Exception("Normal resize margin not restored");
    form.ShowFunctions(tabs.Header,form.Panes[0]);
    var menu=(ContextMenuStrip)tabs.Header.Tag;
    foreach(string text in new[]{"Minimize window","Maximize window","Close workspace","Exit Flow Desk"})
     if(!System.Linq.Enumerable.Any(System.Linq.Enumerable.Cast<ToolStripItem>(menu.Items),item=>item.Text==text))throw new Exception("Missing menu action: "+text);
    menu.Close();
    form.Close();
   }
   Console.WriteLine("PASS 24px tabs, stable content, borderless resize/drag hit tests, maximize/fullscreen restore, window menu");return 0;
  }finally{Store.Root=previous;}
 }
}
