using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Reflection;
[assembly: AssemblyTitle("Flow Desk")]
[assembly: AssemblyProduct("Flow Desk")]
[assembly: AssemblyVersion("0.2.0.0")]

namespace FlowDesk {
public static class Native {
 public delegate bool EnumProc(IntPtr h, IntPtr l);
 [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc p, IntPtr l);
 public delegate void WinEventProc(IntPtr hook,uint evt,IntPtr window,int obj,int child,uint thread,uint time);
 [DllImport("user32.dll")] public static extern IntPtr SetWinEventHook(uint first,uint last,IntPtr module,WinEventProc callback,uint process,uint thread,uint flags);
 [DllImport("user32.dll")] public static extern bool UnhookWinEvent(IntPtr hook);
 [DllImport("user32.dll")] public static extern void NotifyWinEvent(uint evt,IntPtr window,int obj,int child);
 [DllImport("user32.dll")] public static extern bool RedrawWindow(IntPtr window,IntPtr rect,IntPtr region,uint flags);
 [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr h);
 [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
 [DllImport("user32.dll")] public static extern bool IsWindowEnabled(IntPtr h);
 [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr h);
 [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr h,int attribute,out int value,int size);
 [DllImport("dwmapi.dll",EntryPoint="DwmGetWindowAttribute")]public static extern int DwmFrameBounds(IntPtr h,int attribute,out Rect value,int size);
 [DllImport("dwmapi.dll")] public static extern int DwmSetWindowAttribute(IntPtr h,int attribute,ref int value,int size);
 public static void DarkWorkspaceFrame(IntPtr h){int noBorder=unchecked((int)0xfffffffe);DwmSetWindowAttribute(h,34,ref noBorder,4);}
 public static bool Cloaked(IntPtr h){int value;return DwmGetWindowAttribute(h,14,out value,4)==0&&value!=0;}
 [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern int GetWindowText(IntPtr h, StringBuilder s, int n);
 [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
 [DllImport("user32.dll")] public static extern IntPtr GetWindow(IntPtr h, uint cmd);
 [DllImport("user32.dll")] public static extern IntPtr GetParent(IntPtr h);
 [DllImport("user32.dll", SetLastError=true)] public static extern IntPtr SetParent(IntPtr h, IntPtr p);
 [DllImport("user32.dll", EntryPoint="GetWindowLongPtrW")] public static extern IntPtr GetLong(IntPtr h, int index);
 [DllImport("user32.dll", EntryPoint="SetWindowLongPtrW", SetLastError=true)] public static extern IntPtr SetLong(IntPtr h, int index, IntPtr value);
 [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int cmd);
 [DllImport("user32.dll", SetLastError=true)] public static extern bool SetWindowPos(IntPtr h, IntPtr after, int x, int y, int w, int ht, uint flags);
 [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
 [DllImport("user32.dll")] public static extern bool AllowSetForegroundWindow(uint pid);
 [DllImport("user32.dll")] public static extern IntPtr SetFocus(IntPtr h);
 [DllImport("user32.dll")] public static extern bool AttachThreadInput(uint a, uint b, bool attach);
 [DllImport("kernel32.dll")] public static extern uint GetCurrentThreadId();
 [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out Rect r);
 [DllImport("user32.dll")] public static extern bool GetWindowPlacement(IntPtr h, ref Placement p);
 [DllImport("user32.dll")] public static extern bool SetWindowPlacement(IntPtr h, ref Placement p);
 [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern bool SetProp(IntPtr h, string key, IntPtr value);
 [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr GetProp(IntPtr h, string key);
 [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr RemoveProp(IntPtr h, string key);
 [DllImport("user32.dll")] public static extern IntPtr GetWindowDpiAwarenessContext(IntPtr h);
 [DllImport("user32.dll")] public static extern bool AreDpiAwarenessContextsEqual(IntPtr a, IntPtr b);
 [DllImport("user32.dll")] public static extern bool PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
 [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr SendMessageTimeout(IntPtr h, uint m, IntPtr w, IntPtr l, uint f, uint ms, out IntPtr result);
 [DllImport("user32.dll",EntryPoint="SendMessageW")] public static extern IntPtr AdjustTabRect(IntPtr h,uint message,IntPtr w,ref Rect rect);
 public const string Claim="FlowDesk.ManagedWindow.v1";
 public static string Title(IntPtr h) { var s=new StringBuilder(2048); GetWindowText(h,s,s.Capacity); return s.ToString(); }
 public static uint Pid(IntPtr h) { uint p; GetWindowThreadProcessId(h,out p); return p; }
 public static void Check(bool ok, string operation) { if(!ok) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(),operation); }
 public static void FocusWindow(IntPtr h) {
  IntPtr modal=IntPtr.Zero; EnumWindows(delegate(IntPtr w,IntPtr l){ if(IsWindowVisible(w) && GetWindow(w,4)==h) modal=w; return true; },IntPtr.Zero);
  if(modal!=IntPtr.Zero) h=modal;
  uint p;uint target=GetWindowThreadProcessId(h,out p);
  // Never join the host UI input queue to an unrelated process. A blocked
  // application must not block Flow Desk's own window controls or painting.
  SetForegroundWindow(h);if(target==GetCurrentThreadId())SetFocus(h);
 }
}
[Serializable] public struct PointN { public int X,Y; }
[Serializable] public struct Rect { public int Left,Top,Right,Bottom; }
[Serializable] public struct Placement { public int Length,Flags,Show; public PointN Min,Max; public Rect Normal; }
public class Snapshot {
 public int OriginalDesktop=-1;public string OriginalDesktopId="";
 public long Hwnd, Parent, Owner, Style, ExStyle, Started; public int Pid, Host; public Placement Placement; public bool Embedded;
 public bool Valid() { var h=new IntPtr(Hwnd); try { return Native.IsWindow(h) && Native.Pid(h)==Pid && Process.GetProcessById(Pid).StartTime.ToUniversalTime().Ticks==Started && Native.GetProp(h,Native.Claim)==new IntPtr(Host); } catch { return false; } }
 public void Restore() {
  if(!Valid()) return; var h=new IntPtr(Hwnd);
  if(Embedded) Native.SetParent(h,new IntPtr(Parent));
  Native.SetLong(h,-16,new IntPtr(Style)); Native.SetLong(h,-20,new IntPtr(ExStyle)); Native.SetLong(h,-8,new IntPtr(Owner));
  Native.SetWindowPos(h,IntPtr.Zero,0,0,0,0,0x37);
  var p=Placement; Native.SetWindowPlacement(h,ref p); Native.ShowWindow(h,p.Show==2?9:p.Show);
  if(OriginalDesktop>=0){string error;if(!DesktopBridge.Restore(h,Pid,Started,OriginalDesktop,out error,OriginalDesktopId))try{File.AppendAllText(Path.Combine(Store.Root,"desktop-recovery.log"),DateTime.UtcNow.ToString("o")+" Original desktop restoration failed: "+error+Environment.NewLine);}catch{}}
  Native.RemoveProp(h,Native.Claim);
 }
}
public class TabModel { public string Name="", Path="", Title="", Mode="Aligned"; }
public class PanelModel { public List<TabModel> Tabs=new List<TabModel>(); public int Selected; }
public class WorkspaceModel { public string Id=Guid.NewGuid().ToString("N"); public string Name="Workspace"; public bool Saved; public bool Split, Horizontal; public double Ratio=0.5; public PanelModel[] Panels=new PanelModel[]{new PanelModel(),new PanelModel()}; }
public class Session { public string LastWorkspaceId="";public List<WorkspaceModel> Workspaces=new List<WorkspaceModel>();public WorkspaceModel StartupWorkspace(){return Workspaces.FirstOrDefault(w=>w.Id==LastWorkspaceId)??Workspaces.LastOrDefault(w=>w.Panels!=null&&w.Panels.Any(p=>p!=null&&p.Tabs.Count>0))??Workspaces.FirstOrDefault();} }
public static class Store {
 public static string Root=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"FlowDesk");

 public static void Write<T>(string path,T value) { Directory.CreateDirectory(Path.GetDirectoryName(path)); string tmp=path+".tmp"; using(var f=File.Create(tmp)) new XmlSerializer(typeof(T)).Serialize(f,value); if(File.Exists(path)) File.Replace(tmp,path,null); else File.Move(tmp,path); }
 public static T Read<T>(string path) { using(var f=File.OpenRead(path)) return (T)new XmlSerializer(typeof(T)).Deserialize(f); }
 public static double Ratio(double v) { return double.IsNaN(v)||double.IsInfinity(v)?0.5:Math.Max(.15,Math.Min(.85,v)); }
}
public class Candidate {
 public IntPtr H; public string Title,Path,App; public Icon Icon; public bool OtherDesktop;public int ProcessId;public long Started;public int OriginalDesktop=-1;public string OriginalDesktopId="";
 public override string ToString(){ return App+" — "+Title; }
 public static List<Candidate> All(bool includeOtherDesktops=false) {
  var items=new List<Candidate>(); int own=Process.GetCurrentProcess().Id;
  Native.EnumWindows(delegate(IntPtr h,IntPtr l) {
   if(!Native.IsWindowVisible(h)||(!includeOtherDesktops&&Native.Cloaked(h))||Native.GetWindow(h,4)!=IntPtr.Zero||Native.Pid(h)==own||Native.GetProp(h,Native.Claim)!=IntPtr.Zero||(Native.GetLong(h,-20).ToInt64()&0x80)!=0) return true;
   string title=Native.Title(h); if(string.IsNullOrWhiteSpace(title)) return true;
   try { var p=Process.GetProcessById((int)Native.Pid(h)); string path=p.MainModule.FileName;
    if(p.ProcessName.Equals("FlowDesk",StringComparison.OrdinalIgnoreCase)||p.ProcessName=="ApplicationFrameHost"||p.ProcessName=="ShellExperienceHost"||p.ProcessName=="SearchHost"||p.ProcessName=="TextInputHost"||p.ProcessName=="SystemSettings"||p.ProcessName=="codex-computer-use"||title=="Program Manager") return true;
    Icon icon=null; IntPtr result; Native.SendMessageTimeout(h,0x7f,new IntPtr(2),IntPtr.Zero,2,100,out result); if(result!=IntPtr.Zero) icon=(Icon)Icon.FromHandle(result).Clone();
    if(icon==null) try { icon=Icon.ExtractAssociatedIcon(path); } catch {}
    bool other=Native.Cloaked(h)&&DesktopAccess.IsOther(h);if(Native.Cloaked(h)&&!other)return true;
    items.Add(new Candidate{H=h,Title=title,Path=path,App=p.ProcessName,Icon=icon,OtherDesktop=other,ProcessId=p.Id,Started=p.StartTime.ToUniversalTime().Ticks});
   } catch {} return true;
  },IntPtr.Zero); return items.OrderBy(x=>x.App).ThenBy(x=>x.Title).ToList();
 }
}
public class ManagedWindow {
 public static Rectangle FillScreenEdges(Rectangle desired,Rectangle screen,Rect outer,Rect visible){int left=Math.Max(0,visible.Left-outer.Left),right=Math.Max(0,outer.Right-visible.Right),bottom=Math.Max(0,outer.Bottom-visible.Bottom);if(left>32||right>32||bottom>32)return desired;if(desired.Left!=screen.Left)left=0;if(desired.Right!=screen.Right)right=0;if(desired.Bottom!=screen.Bottom)bottom=0;return new Rectangle(desired.X-left,desired.Y,desired.Width+left+right,desired.Height+bottom);}
 Rectangle? requestedBounds;Rect acceptedBounds;
 public Snapshot State; public TabModel Model; public string Journal; public IntPtr H {get{return new IntPtr(State.Hwnd);}}
 public ManagedWindow(Candidate c,TabModel model,Control surface,Form host) { if(c.ProcessId!=0&&(Native.Pid(c.H)!=c.ProcessId||(c.Started!=0&&Process.GetProcessById(c.ProcessId).StartTime.ToUniversalTime().Ticks!=c.Started)))throw new Exception("Selected window changed; Refresh and select it again.");
  if(Native.Cloaked(c.H))throw new Exception("This window is still hidden on another desktop. Move it here in Task View, then Refresh and add it again.");
  if(Native.GetProp(c.H,Native.Claim)!=IntPtr.Zero) throw new Exception("This window already belongs to a workspace.");
  var proc=Process.GetProcessById((int)Native.Pid(c.H));
  if(model.Mode=="Embedded" && !Native.AreDpiAwarenessContextsEqual(Native.GetWindowDpiAwarenessContext(c.H),Native.GetWindowDpiAwarenessContext(surface.Handle))) throw new Exception("Embedding requires matching DPI awareness. Use Aligned mode to avoid resetting the application's DPI context.");
  var placement=new Placement{Length=Marshal.SizeOf(typeof(Placement))}; Native.Check(Native.GetWindowPlacement(c.H,ref placement),"Read placement");
  State=new Snapshot{Hwnd=c.H.ToInt64(),Parent=Native.GetParent(c.H).ToInt64(),Owner=Native.GetWindow(c.H,4).ToInt64(),Style=Native.GetLong(c.H,-16).ToInt64(),ExStyle=Native.GetLong(c.H,-20).ToInt64(),Placement=placement,Pid=proc.Id,Started=proc.StartTime.ToUniversalTime().Ticks,Host=Process.GetCurrentProcess().Id,OriginalDesktop=c.OriginalDesktop,OriginalDesktopId=c.OriginalDesktopId,Embedded=model.Mode=="Embedded"}; Model=model;
  Native.Check(Native.SetProp(c.H,Native.Claim,new IntPtr(State.Host)),"Cannot manage window (it may run as administrator)");
  Journal=Path.Combine(Store.Root,"recovery",Guid.NewGuid()+".xml");
  try {
   Store.Write(Journal,State);
   string ready=Journal+".ready";
   Process.Start(new ProcessStartInfo(Application.ExecutablePath,"--guard \""+Journal+"\""){UseShellExecute=false,CreateNoWindow=true});
   var clock=Stopwatch.StartNew(); while(!File.Exists(ready) && clock.ElapsedMilliseconds<4000) System.Threading.Thread.Sleep(20);
   if(!File.Exists(ready)) throw new Exception("Recovery guardian did not start. Window left unchanged.");
   Native.ShowWindow(c.H,9);
   if(State.Embedded) {
    Native.SetLong(c.H,-16,new IntPtr((State.Style & ~unchecked((long)0x80C40000)) | 0x40000000));
    Native.SetParent(c.H,surface.Handle); if(Native.GetParent(c.H)!=surface.Handle) throw new Exception("SetParent failed; choose Aligned mode.");
    Native.PostMessage(c.H,0x127,new IntPtr(3),IntPtr.Zero);
   } else {
    Native.SetLong(c.H,-8,host.Handle); if(Native.GetWindow(c.H,4)!=host.Handle) throw new Exception("Cannot set workspace ownership (privilege restriction).");
   }
  } catch { State.Restore(); Cleanup(); throw; }
 }
 public void Cleanup(){ try { if(File.Exists(Journal)) File.Delete(Journal); }catch{} }
 public void Detach(){ State.Restore(); Cleanup(); }
 public bool Alive {get {return State.Valid();}}
 public void Place(Control surface,bool visible) {
  if(!Alive) return;
  if(!visible){if(Native.IsWindowVisible(H))Native.ShowWindow(H,0);return;}
  Rectangle r=State.Embedded?surface.ClientRectangle:surface.RectangleToScreen(surface.ClientRectangle);
  Rect actual;Native.GetWindowRect(H,out actual);
  Rectangle screen=surface.RectangleToScreen(surface.ClientRectangle);
  var workspace=surface.FindForm() as Workspace;Rect frame;
  if(!State.Embedded&&workspace!=null&&workspace.IsFullScreen&&Native.DwmFrameBounds(H,9,out frame,Marshal.SizeOf(typeof(Rect)))==0&&frame.Right>frame.Left&&frame.Bottom>frame.Top)r=FillScreenEdges(screen,Screen.FromRectangle(screen).Bounds,actual,frame);
  bool unchangedConstraint=requestedBounds.HasValue&&requestedBounds.Value==screen&&actual.Equals(acceptedBounds);
  if(!unchangedConstraint&&(actual.Left!=r.X||actual.Top!=r.Y||actual.Right-actual.Left!=r.Width||actual.Bottom-actual.Top!=r.Height)){
   Native.SetWindowPos(H,IntPtr.Zero,r.X,r.Y,Math.Max(1,r.Width),Math.Max(1,r.Height),0x14);
   Native.GetWindowRect(H,out acceptedBounds);requestedBounds=screen;
  }
  // Position a hidden tab before revealing it, avoiding a frame at its old bounds.
  if(!Native.IsWindowVisible(H))Native.SetWindowPos(H,IntPtr.Zero,0,0,0,0,0x57);
 }
}
public static class Theme {
 // Closed runs inside ToolStrip's click dispatch. Keep the menu alive until
 // the next opening (after dispatch) or until its owning button is disposed.
 public static void KeepMenu(Control anchor,ContextMenuStrip menu) {
  var previous=anchor.Tag as ContextMenuStrip;
  if(previous!=null)previous.Dispose();
  else anchor.Disposed+=delegate{var owned=anchor.Tag as ContextMenuStrip;if(owned!=null)owned.Dispose();anchor.Tag=null;};
  anchor.Tag=menu;
 }
 public static Color Bg=Color.FromArgb(23,25,31), Bar=Color.FromArgb(34,37,46), Ink=Color.FromArgb(225,230,240), Accent=Color.FromArgb(93,151,255);
 public static Button Button(string text,EventHandler click) { var b=new Button{Text=text,AutoSize=true,Height=30,FlatStyle=FlatStyle.Flat,BackColor=Bar,ForeColor=Ink,Margin=new Padding(3)}; b.FlatAppearance.BorderSize=0;b.Click+=click;return b; }
 public static void Style(Form f) {f.BackColor=Bg;f.ForeColor=Ink;f.Font=new Font("Segoe UI",9);f.StartPosition=FormStartPosition.CenterScreen;}
 public static string Ask(string label,string initial) { using(var f=new Form{Text=label,Size=new Size(420,150)}) {Style(f);var t=new TextBox{Text=initial,Dock=DockStyle.Top};f.Controls.Add(t);var b=Button("Save",delegate{f.DialogResult=DialogResult.OK;});b.Dock=DockStyle.Bottom;f.Controls.Add(b);f.AcceptButton=b; return f.ShowDialog()==DialogResult.OK?t.Text:null;} }
}
public class Picker:Form {
 bool moveBusy;
 Label desktopHelp=new Label{Dock=DockStyle.Bottom,Height=48,Padding=new Padding(6),ForeColor=Color.FromArgb(160,170,190),Text="Selected windows are moved here. Detaching returns them to their original desktop."};
 public int SkippedCount;
 async void AddSelected(){if(moveBusy)return;var checkedWindows=list.CheckedItems.Cast<ListViewItem>().Select(x=>(Candidate)x.Tag).ToArray();if(checkedWindows.Length==0)return;IntPtr host=Owner==null?Handle:Owner.Handle;moveBusy=true;list.Enabled=false;desktopHelp.Text="Moving selected windows to this desktop…";try{var plan=await System.Threading.Tasks.Task.Run(()=>WindowSelection.Build(checkedWindows,delegate(Candidate c){if(!Native.Cloaked(c.H))return true;string reason;return DesktopAccess.BringCandidate(c,host,out reason);}));Selected=plan.Allowed;SkippedCount=plan.Skipped.Count;if(SkippedCount>0)desktopHelp.Text="Could not move "+SkippedCount+" window(s). Move them here in Task View, then Refresh.";}catch(Exception e){desktopHelp.Text=e.Message;}finally{moveBusy=false;list.Enabled=true;}if(Selected.Count>0)DialogResult=DialogResult.OK;}
 protected override void OnFormClosing(FormClosingEventArgs e){if(moveBusy){e.Cancel=true;desktopHelp.Text="Finishing the desktop move…";}base.OnFormClosing(e);}
 CheckBox otherDesktops=new CheckBox{Text="Include windows from other virtual desktops",Checked=true,Dock=DockStyle.Top,Height=28};
 ListView list=new ListView{Dock=DockStyle.Fill,View=View.Details,CheckBoxes=true,FullRowSelect=true,HideSelection=false};
 TextBox search=new TextBox{Dock=DockStyle.Top}; ComboBox mode=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Width=130}; List<Candidate> all;
 public List<Candidate> Selected=new List<Candidate>(); public string Mode {get{return mode.Text;}}
 public Picker() {
  Text="Add window — select running windows";Size=new Size(820,510);Theme.Style(this);list.BackColor=Theme.Bg;list.ForeColor=Theme.Ink;
  list.Columns.Add("Application",150);list.Columns.Add("Window title",430);list.Columns.Add("Desktop",140);var images=new ImageList{ImageSize=new Size(20,20),ColorDepth=ColorDepth.Depth32Bit};list.SmallImageList=images;
  Controls.Add(list);Controls.Add(desktopHelp);Controls.Add(otherDesktops);Controls.Add(search);var bar=new FlowLayoutPanel{Dock=DockStyle.Bottom,Height=44};mode.Items.AddRange(new object[]{"Aligned","Embedded"});mode.SelectedIndex=0;bar.Controls.Add(mode);bar.Controls.Add(Theme.Button("Add selected",delegate{AddSelected();}));bar.Controls.Add(Theme.Button("Refresh",delegate{Reload();}));Controls.Add(bar);
  search.TextChanged+=delegate{Render();};otherDesktops.CheckedChanged+=delegate{Reload();};Reload();
 }
 void Reload(){all=Candidate.All(otherDesktops.Checked);Render();}
 void Render(){list.Items.Clear();list.SmallImageList.Images.Clear();foreach(var c in all.Where(c=>c.ToString().IndexOf(search.Text,StringComparison.OrdinalIgnoreCase)>=0)) {int idx=-1;if(c.Icon!=null){list.SmallImageList.Images.Add(c.Icon);idx=list.SmallImageList.Images.Count-1;}var item=new ListViewItem(new[]{c.App,c.Title,c.OtherDesktop?"Other desktop":"Current desktop"},idx){Tag=c};list.Items.Add(item);}}
}
public class DarkTabs:TabControl {
 public Control Header;int scroll;ToolTip tip=new ToolTip();Control viewport;bool fitting,fitQueued;int nativeContentTop;
 public event EventHandler MenuRequested;
 public DarkTabs(){
  Header=new TabHeader(this){Height=24,Dock=DockStyle.Top,AllowDrop=true,BackColor=Theme.Bar};
  DrawMode=TabDrawMode.OwnerDrawFixed;
  Header.MouseDown+=delegate(object s,MouseEventArgs e){var host=Header.FindForm() as Workspace;int hit=host==null?1:host.WindowHitTest(Header.PointToScreen(e.Location));if(e.Button==MouseButtons.Left&&(hit==2||(hit>=10&&hit<=17))){if(hit==2&&e.Clicks==2)host.ToggleMaximize();else host.BeginFrameDrag(hit);return;}if(e.X<30){if(e.Button==MouseButtons.Left&&MenuRequested!=null)MenuRequested(this,EventArgs.Empty);return;}for(int i=0;i<TabCount;i++)if(GetTabRect(i).Contains(e.Location)){if(e.Button==MouseButtons.Left)SelectedIndex=i;break;}OnMouseDown(e);};
  Header.MouseMove+=delegate(object s,MouseEventArgs e){OnMouseMove(e);};
  Header.MouseUp+=delegate(object s,MouseEventArgs e){OnMouseUp(e);};
  Header.MouseWheel+=delegate(object s,MouseEventArgs e){scroll=Math.Max(0,scroll+(e.Delta<0?100:-100));Header.Invalidate();};
  Header.DragEnter+=delegate(object s,DragEventArgs e){OnDragEnter(e);};Header.DragDrop+=delegate(object s,DragEventArgs e){OnDragDrop(e);};
  SelectedIndexChanged+=delegate{if(SelectedIndex>=0){Rectangle r=GetTabRect(SelectedIndex);if(r.Right>Header.Width)scroll+=r.Right-Header.Width;else if(r.Left<30)scroll=Math.Max(0,scroll+r.Left-30);}Header.Invalidate();};
  ControlAdded+=delegate(object s,ControlEventArgs e){e.Control.TextChanged+=delegate{Header.Invalidate();QueueFit();};Header.Invalidate();QueueFit();};ControlRemoved+=delegate{Header.Invalidate();QueueFit();};
  FontChanged+=delegate{QueueFit();};SelectedIndexChanged+=delegate{QueueFit();};
 }
 int TabWidth(TabPage page){return Math.Max(100,Math.Min(185,TextRenderer.MeasureText(page.Text,Header.Font).Width+34));}
 public new Rectangle GetTabRect(int index){int x=30-scroll;for(int i=0;i<index;i++)x+=TabWidth(TabPages[i]);return new Rectangle(x,0,TabWidth(TabPages[index]),24);}
 public Point HeaderPoint(Point screen){return Header.PointToClient(screen);}
 public void FitViewport(Control owner){viewport=owner;Refit();QueueFit();}
 void QueueFit(){if(fitQueued||!IsHandleCreated||IsDisposed)return;fitQueued=true;BeginInvoke((Action)delegate{fitQueued=false;if(!IsDisposed)Refit();});}
 void Refit(){if(fitting||viewport==null||viewport.IsDisposed||!IsHandleCreated)return;fitting=true;try{
  var rect=new Rect{Right=ClientSize.Width,Bottom=ClientSize.Height};Native.AdjustTabRect(Handle,0x1328,IntPtr.Zero,ref rect);
  int left=Math.Max(0,rect.Left),top=Math.Max(0,rect.Top),right=Math.Max(0,ClientSize.Width-rect.Right),bottom=Math.Max(0,ClientSize.Height-rect.Bottom);
  Bounds=new Rectangle(-left,-top,viewport.ClientSize.Width+left+right,viewport.ClientSize.Height+top+bottom);
  rect=new Rect{Right=ClientSize.Width,Bottom=ClientSize.Height};Native.AdjustTabRect(Handle,0x1328,IntPtr.Zero,ref rect);nativeContentTop=rect.Top;
  Rectangle content=new Rectangle(rect.Left,rect.Top,Math.Max(0,rect.Right-rect.Left),Math.Max(0,rect.Bottom-rect.Top));if(SelectedTab!=null){SelectedTab.UseVisualStyleBackColor=false;SelectedTab.BackColor=Theme.Bg;content=Rectangle.Intersect(content,SelectedTab.Bounds);}
  var previous=Region;Region=new Region(content);if(previous!=null)previous.Dispose();
 }finally{fitting=false;}}
 protected override void WndProc(ref Message m){if(m.Msg==0x130d){m.Result=new IntPtr(-1);return;}if((m.Msg==0x201||m.Msg==0x203||m.Msg==0x204||m.Msg==0x206)&&((short)((m.LParam.ToInt64()>>16)&0xffff))<nativeContentTop){m.Result=IntPtr.Zero;return;}base.WndProc(ref m);if(m.Msg==0x30||m.Msg==0x2e0)QueueFit();}
 protected override void Dispose(bool disposing){if(disposing)tip.Dispose();base.Dispose(disposing);}
 class TabHeader:Control {
  DarkTabs tabs;
  public TabHeader(DarkTabs owner){tabs=owner;Font=new Font("Segoe UI",8.25f);SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.UserPaint,true);}
  protected override void WndProc(ref Message m){if(m.Msg==0x84){m.Result=new IntPtr(1);return;}base.WndProc(ref m);}
  protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);var g=e.Graphics;g.SetClip(new Rectangle(30,0,Math.Max(0,Width-30),Height));for(int i=0;i<tabs.TabCount;i++){var r=tabs.GetTabRect(i);if(r.Right<30||r.Left>Width)continue;var page=tabs.TabPages[i];using(var b=new SolidBrush(i==tabs.SelectedIndex?Theme.Bg:Theme.Bar))g.FillRectangle(b,r);int x=r.X+6;if(page.ImageIndex>=0&&tabs.ImageList!=null&&page.ImageIndex<tabs.ImageList.Images.Count){g.DrawImage(tabs.ImageList.Images[page.ImageIndex],x,6,12,12);x+=17;}TextRenderer.DrawText(g,page.Text.TrimEnd(' ','×'),Font,new Rectangle(x,0,Math.Max(0,r.Right-x-22),Height),Theme.Ink,TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis|TextFormatFlags.SingleLine);TextRenderer.DrawText(g,"×",Font,new Rectangle(r.Right-21,0,20,Height),Theme.Ink,TextFormatFlags.VerticalCenter|TextFormatFlags.HorizontalCenter);if(i==tabs.SelectedIndex)using(var pen=new Pen(Theme.Accent,2))g.DrawLine(pen,r.Left,23,r.Right,23);}g.ResetClip();using(var b=new SolidBrush(Theme.Bar))g.FillRectangle(b,0,0,30,24);using(var pen=new Pen(Theme.Ink,1.4f)){for(int y=7;y<=17;y+=5)g.DrawLine(pen,9,y,21,y);}}
 }
}
public class EmptyWorkspace:Panel {
 Label title=new Label{Text="Your workspace is empty",AutoSize=false,Height=30,TextAlign=ContentAlignment.MiddleCenter,ForeColor=Theme.Ink,Font=new Font("Segoe UI",12)};
 Label hint=new Label{Text="Add an open window or start an app",AutoSize=false,Height=24,TextAlign=ContentAlignment.MiddleCenter,ForeColor=Color.FromArgb(140,148,165)};
 Button add,run;
 public EmptyWorkspace(EventHandler onAdd,EventHandler onRun){Dock=DockStyle.Fill;BackColor=Theme.Bg;SetStyle(ControlStyles.OptimizedDoubleBuffer|ControlStyles.AllPaintingInWmPaint|ControlStyles.UserPaint,true);add=Theme.Button("+ Add window",onAdd);run=Theme.Button("Run app",onRun);Controls.Add(title);Controls.Add(hint);Controls.Add(add);Controls.Add(run);Resize+=delegate{Arrange();Invalidate();};}
 void Arrange(){int y=Math.Max(80,Height/2-20);title.SetBounds(0,y,Width,30);hint.SetBounds(0,y+34,Width,24);int total=add.PreferredSize.Width+run.PreferredSize.Width+12;int x=Math.Max(0,(Width-total)/2);add.SetBounds(x,y+76,add.PreferredSize.Width,32);run.SetBounds(x+add.Width+12,y+76,run.PreferredSize.Width,32);}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);int x=Width/2-28,y=Math.Max(80,Height/2-20)-82;using(var pen=new Pen(Color.FromArgb(85,111,156),2)){e.Graphics.DrawRectangle(pen,x,y,56,46);e.Graphics.DrawLine(pen,x+18,y,x+18,y+46);e.Graphics.DrawLine(pen,x+18,y+22,x+56,y+22);}using(var brush=new SolidBrush(Theme.Accent))e.Graphics.FillRectangle(brush,x+5,y+6,7,7);}
}
public class Pane:Panel {
 bool foregroundSelection;
 public bool SelectForeground(IntPtr foreground){for(int depth=0;foreground!=IntPtr.Zero&&depth<8;depth++,foreground=Native.GetWindow(foreground,4)){var match=Windows.FirstOrDefault(p=>p.Value.H==foreground);if(match.Key!=null){Host.Active=this;foregroundSelection=true;try{Tabs.SelectedTab=match.Key;}finally{foregroundSelection=false;}return true;}}return false;}
 public Workspace Host; public PanelModel Model; public DarkTabs Tabs=new DarkTabs{Dock=DockStyle.Fill,AllowDrop=true}; public Dictionary<TabPage,ManagedWindow> Windows=new Dictionary<TabPage,ManagedWindow>();
 ImageList icons=new ImageList{ImageSize=new Size(16,16),ColorDepth=ColorDepth.Depth32Bit}; TabPage drag;Point dragOrigin;
 public Pane(Workspace host,PanelModel model) {
  Host=host;Model=model;Dock=DockStyle.Fill;BackColor=Theme.Bg;Tabs.ImageList=icons;var viewport=new Panel{Dock=DockStyle.Fill,BackColor=Theme.Bg};Tabs.Dock=DockStyle.None;viewport.Controls.Add(Tabs);Controls.Add(viewport);Controls.Add(Tabs.Header);viewport.Resize+=delegate{Tabs.FitViewport(viewport);};Tabs.HandleCreated+=delegate{Tabs.FitViewport(viewport);};Tabs.MenuRequested+=delegate{Host.Active=this;Host.ShowFunctions(Tabs.Header,this);};
  var empty=new EmptyWorkspace(delegate{Add();},delegate{RunApp();});viewport.Controls.Add(empty);Action emptyState=delegate{bool isEmpty=Tabs.TabCount==0;Tabs.Visible=!isEmpty;empty.Visible=isEmpty;if(isEmpty)empty.BringToFront();};Tabs.ControlAdded+=delegate{emptyState();};Tabs.ControlRemoved+=delegate{emptyState();};emptyState();
  if(Tabs.TabCount>0)Tabs.SelectedIndex=Math.Max(0,Math.Min(model.Selected,Tabs.TabCount-1));
  Tabs.Enter+=delegate{Host.Active=this;};
  Tabs.SelectedIndexChanged+=delegate{Model.Selected=Tabs.SelectedIndex;Sync(!foregroundSelection);Host.Save();};
  Tabs.MouseDown+=delegate(object s,MouseEventArgs e){Host.Active=this;dragOrigin=e.Location;for(int i=0;i<Tabs.TabCount;i++)if(Tabs.GetTabRect(i).Contains(e.Location)){drag=Tabs.TabPages[i];if(e.Button==MouseButtons.Right && Windows.ContainsKey(drag)) Menu(drag).Show(Tabs,e.Location);if(e.Button==MouseButtons.Left && e.X>Tabs.GetTabRect(i).Right-23){var page=drag;drag=null;BeginInvoke((Action)delegate{if(Windows.ContainsKey(page))Remove(page,false);else {Tabs.TabPages.Remove(page);UpdateModel();Host.Save();}});}break;}};
  Tabs.MouseMove+=delegate(object s,MouseEventArgs e){if(e.Button==MouseButtons.Left&&drag!=null&&(Math.Abs(e.X-dragOrigin.X)>SystemInformation.DragSize.Width/2||Math.Abs(e.Y-dragOrigin.Y)>SystemInformation.DragSize.Height/2)){var p=drag;drag=null;Tabs.DoDragDrop(p,DragDropEffects.Move);}};
  Tabs.DragEnter+=delegate(object s,DragEventArgs e){if(e.Data.GetDataPresent(typeof(TabPage)))e.Effect=DragDropEffects.Move;};
  Tabs.DragDrop+=delegate(object s,DragEventArgs e){var page=e.Data.GetData(typeof(TabPage)) as TabPage; if(page==null)return;Pane source=Program.Context.Forms.SelectMany(f=>f.Panes).FirstOrDefault(p=>p.Windows.ContainsKey(page));if(source==null)return;var win=source.Windows[page];if(win.State.Embedded&&source!=this){MessageBox.Show("Detach and re-add embedded windows to change panel.");return;}source.Windows.Remove(page);source.Tabs.TabPages.Remove(page);int at=Tabs.TabCount;Point pos=Tabs.HeaderPoint(new Point(e.X,e.Y));for(int i=0;i<Tabs.TabCount;i++)if(Tabs.GetTabRect(i).Contains(pos)){at=i;break;}Windows[page]=win;Tabs.TabPages.Insert(at,page);if(!win.State.Embedded)Native.SetLong(win.H,-8,Host.Handle);Tabs.SelectedTab=page;source.UpdateModel();UpdateModel();source.Sync(false);Sync(true);Host.Save();};
  Tabs.Resize+=delegate{Sync(false);};
 }
 public void Add(){Host.Active=this;using(var p=new Picker()){if(p.ShowDialog(Host)!=DialogResult.OK)return;foreach(var c in p.Selected){try{Attach(c,new TabModel{Name=c.Title,Title=c.Title,Path=c.Path,Mode=p.Mode});}catch(Exception ex){DesktopBridge.RestoreCandidate(c);MessageBox.Show(Host,ex.Message,"Cannot add window");}}}Host.Save();}
 public void RunApp(){Host.Active=this;using(var dialog=new RunAppDialog()){if(dialog.ShowDialog(Host)!=DialogResult.OK||dialog.Selected==null)return;var c=dialog.Selected;try{Attach(c,new TabModel{Name=c.Title,Title=c.Title,Path=c.Path,Mode=dialog.Mode});}catch(Exception e){MessageBox.Show(Host,e.Message,"Cannot add launched window");}}Host.Save();}
 public void Attach(Candidate c,TabModel model){var pending=Tabs.TabPages.Cast<TabPage>().FirstOrDefault(p=>p.Tag==model); var page=new TabPage(model.Name+"   ×"){Tag=model,BackColor=Theme.Bg,ToolTipText=model.Mode+" · right click for actions"};if(c.Icon!=null){icons.Images.Add(c.Icon);page.ImageIndex=icons.Images.Count-1;}Tabs.TabPages.Add(page);try{var w=new ManagedWindow(c,model,page,Host);Windows.Add(page,w);if(pending!=null){int at=Tabs.TabPages.IndexOf(pending);Tabs.TabPages.Remove(pending);Tabs.TabPages.Remove(page);Tabs.TabPages.Insert(at,page);pending.Dispose();}Tabs.SelectedTab=page;UpdateModel();Sync(true);}catch{Tabs.TabPages.Remove(page);page.Dispose();throw;}}
 ContextMenuStrip Menu(TabPage page){var menu=new ContextMenuStrip();menu.Items.Add("Detach — keep application open",null,delegate{Remove(page,false);});menu.Items.Add("Close application window…",null,delegate{if(MessageBox.Show(Host,"Send a normal close request to this application window? Unsaved changes may prompt.","Close application",MessageBoxButtons.YesNo)==DialogResult.Yes)Remove(page,true);});menu.Items.Add("Rename tab",null,delegate{string n=Theme.Ask("Tab name",Windows[page].Model.Name);if(!string.IsNullOrWhiteSpace(n)){Windows[page].Model.Name=n;page.Text=n+"   ×";Host.Save();}});return menu;}
 public void Remove(TabPage page,bool close){ManagedWindow w;if(!Windows.TryGetValue(page,out w))return;w.Detach();Windows.Remove(page);Tabs.TabPages.Remove(page);page.Dispose();if(close&&Native.IsWindow(w.H))Native.PostMessage(w.H,0x10,IntPtr.Zero,IntPtr.Zero);UpdateModel();Host.Save();}
 public void UpdateModel(){Model.Tabs=Tabs.TabPages.Cast<TabPage>().Select(p=>(TabModel)p.Tag).ToList();Model.Selected=Tabs.SelectedIndex;}
 public void Sync(bool focus){if(Host.IsHandleCreated&&Native.Cloaked(Host.Handle))return;foreach(var pair in Windows.OrderBy(p=>p.Key==Tabs.SelectedTab?0:1).ToArray()){if(!pair.Value.Alive){Windows.Remove(pair.Key);Tabs.TabPages.Remove(pair.Key);pair.Value.Cleanup();UpdateModel();Host.Save();continue;}bool show=Visible&&Host.Visible&&Host.Enabled&&Host.WindowState!=FormWindowState.Minimized&&pair.Key==Tabs.SelectedTab;pair.Value.Place(pair.Key,show);if(show&&focus)Native.FocusWindow(pair.Value.H);}}
 public void Release(){foreach(var w in Windows.Values)w.Detach();Windows.Clear();}
}
public class Workspace:Form {
 bool wasCloaked;
 public void RefreshDesktopState(IntPtr foreground,bool shellReturn){if(IsDisposed||!IsHandleCreated)return;bool cloaked=Native.Cloaked(Handle);if(cloaked){wasCloaked=true;return;}bool returned=wasCloaked;wasCloaked=false;if(WindowState==FormWindowState.Minimized)return;bool related=foreground==Handle;foreach(var pane in Panes)related=pane.SelectForeground(foreground)||related;if(!related&&!returned&&!shellReturn)return;
  long style=Native.GetLong(Handle,-16).ToInt64();if((style&0x00c40000)!=0){Native.SetLong(Handle,-16,new IntPtr(style&~0x00c40000));Native.SetWindowPos(Handle,IntPtr.Zero,0,0,0,0,0x37);}Native.DarkWorkspaceFrame(Handle);foreach(var pane in Panes){pane.Tabs.FitViewport(pane.Tabs.Parent);pane.Tabs.Header.Invalidate();}Sync(false);Native.RedrawWindow(Handle,IntPtr.Zero,IntPtr.Zero,0x481);
 }
 public WorkspaceModel Model; public List<Pane> Panes=new List<Pane>();public Pane Active;SplitContainer split=new SplitContainer{Dock=DockStyle.Fill,SplitterWidth=7,BackColor=Theme.Accent};bool loading=true,expanded,closing;
 bool fullScreen;Rectangle beforeFullScreen;FormWindowState beforeFullScreenState;Rectangle? beforeMaximize;
 public bool IsFullScreen {get{return fullScreen;}}
 int dragHit;Point dragStart;Rectangle dragBounds;
 public static Rectangle DragBounds(Rectangle original,Point delta,int hit,Size minimum){int l=original.Left,t=original.Top,r=original.Right,b=original.Bottom;if(hit==2)return new Rectangle(original.Location+new Size(delta),original.Size);if(hit==10||hit==13||hit==16)l=Math.Min(original.Right-minimum.Width,l+delta.X);if(hit==11||hit==14||hit==17)r=Math.Max(original.Left+minimum.Width,r+delta.X);if(hit==12||hit==13||hit==14)t=Math.Min(original.Bottom-minimum.Height,t+delta.Y);if(hit==15||hit==16||hit==17)b=Math.Max(original.Top+minimum.Height,b+delta.Y);return Rectangle.FromLTRB(l,t,r,b);}
 void StartWindowDrag(int hit){if(fullScreen)return;if(hit==2&&WindowState==FormWindowState.Maximized){ToggleMaximize();}if(WindowState!=FormWindowState.Normal)return;dragHit=hit;dragStart=Cursor.Position;dragBounds=Bounds;Capture=true;}
 public void BeginFrameDrag(int hit){StartWindowDrag(hit);}
 protected override void OnMouseCaptureChanged(EventArgs e){base.OnMouseCaptureChanged(e);if(!Capture)dragHit=0;}
 public Workspace(WorkspaceModel model){Model=model;Text="Flow Desk · "+model.Name;FormBorderStyle=FormBorderStyle.None;Padding=new Padding(3);Size=new Size(1200,800);MinimumSize=new Size(650,400);Theme.Style(this);Icon=Icon.ExtractAssociatedIcon(Application.ExecutablePath);KeyPreview=true;
  Controls.Add(split);
  if(model.Panels==null||model.Panels.Length!=2)model.Panels=new PanelModel[]{new PanelModel(),new PanelModel()};for(int i=0;i<2;i++)Panes.Add(new Pane(this,model.Panels[i]));Active=Panes[0];split.Panel1.Controls.Add(Panes[0]);split.Panel2.Controls.Add(Panes[1]);
  split.SplitterMoved+=delegate{if(!loading&&!expanded){int length=Model.Horizontal?split.Height:split.Width;Model.Ratio=Store.Ratio((double)split.SplitterDistance/Math.Max(1,length-split.SplitterWidth));Save();}Sync(false);};
  Move+=delegate{Sync(false);};Resize+=delegate{Sync(false);};Activated+=delegate{Sync(false);};VisibleChanged+=delegate{Sync(false);};
  Shown+=delegate{Native.DarkWorkspaceFrame(Handle);ApplyLayout();loading=false;};FormClosing+=delegate{closing=true;Save();foreach(var p in Panes)p.Release();};
  var timer=new System.Windows.Forms.Timer{Interval=500};timer.Tick+=delegate{if(!closing){if(Native.Cloaked(Handle)){wasCloaked=true;return;}if(wasCloaked)RefreshDesktopState(Native.GetForegroundWindow(),true);else Sync(false);}};timer.Start();FormClosed+=delegate{timer.Dispose();Program.Context.Closed(this);};
 }
 void SetSplit(bool horizontal){Model.Split=true;Model.Horizontal=horizontal;expanded=false;ApplyLayout();Save();}
 public void ToggleMaximize(){if(fullScreen)ToggleFullScreen();if(WindowState==FormWindowState.Maximized){WindowState=FormWindowState.Normal;if(beforeMaximize.HasValue)Bounds=beforeMaximize.Value;}else{beforeMaximize=Bounds;MaximizedBounds=Screen.FromRectangle(Bounds).WorkingArea;WindowState=FormWindowState.Maximized;}}
 public void ToggleFullScreen(){if(!fullScreen){beforeFullScreenState=WindowState;beforeFullScreen=WindowState==FormWindowState.Normal?Bounds:(beforeMaximize??RestoreBounds);WindowState=FormWindowState.Normal;fullScreen=true;Padding=Padding.Empty;Bounds=Screen.FromRectangle(Bounds).Bounds;}else{fullScreen=false;Padding=new Padding(3);Bounds=beforeFullScreen;WindowState=beforeFullScreenState;}Sync(false);}
 void QueueWindowAction(Action action){BeginInvoke((Action)delegate{if(!IsDisposed)action();});}
 public int WindowHitTest(Point screen){
  if(WindowState==FormWindowState.Normal&&!fullScreen){Point p=PointToClient(screen);const int edge=6;bool left=p.X<edge,right=p.X>=ClientSize.Width-edge,top=p.Y<edge,bottom=p.Y>=ClientSize.Height-edge;if(top&&left)return 13;if(top&&right)return 14;if(bottom&&left)return 16;if(bottom&&right)return 17;if(left)return 10;if(right)return 11;if(top)return 12;if(bottom)return 15;}
  foreach(var pane in Panes){var header=pane.Tabs.Header;if(!header.Visible||!header.RectangleToScreen(header.ClientRectangle).Contains(screen))continue;Point p=header.PointToClient(screen);if(p.X<30)return 1;for(int i=0;i<pane.Tabs.TabCount;i++)if(pane.Tabs.GetTabRect(i).Contains(p))return 1;return 2;}
  return 1;
 }

 public void ShowFunctions(Control anchor,Pane pane){var m=new ContextMenuStrip();m.Items.Add("Add window…",null,delegate{pane.Add();});m.Items.Add("Run app…",null,delegate{pane.RunApp();});var splitItem=new ToolStripMenuItem("Split");splitItem.DropDownItems.Add("Left / right",null,delegate{SetSplit(false);});splitItem.DropDownItems.Add("Top / bottom",null,delegate{SetSplit(true);});splitItem.DropDownItems.Add("Single panel",null,delegate{if(Panes[1].Tabs.TabCount>0){MessageBox.Show("Detach or move the second panel's tabs first.");return;}Model.Split=false;Active=Panes[0];ApplyLayout();Save();});m.Items.Add(splitItem);m.Items.Add("Expand / restore panel",null,delegate{Expand(pane);});if(Model.Saved)m.Items.Add("Restore saved tabs",null,delegate{RestoreTabs();});var workspaces=new ToolStripMenuItem("Workspaces"){DropDown=WorkspaceMenu()};m.Items.Add(workspaces);m.Items.Add(new ToolStripSeparator());m.Items.Add("Minimize window",null,delegate{QueueWindowAction(delegate{WindowState=FormWindowState.Minimized;});});m.Items.Add(WindowState==FormWindowState.Maximized?"Restore window":"Maximize window",null,delegate{QueueWindowAction(ToggleMaximize);});var fullscreen=new ToolStripMenuItem("Full screen (F11)"){Checked=fullScreen};fullscreen.Click+=delegate{QueueWindowAction(ToggleFullScreen);};m.Items.Add(fullscreen);m.Items.Add("Close workspace",null,delegate{QueueWindowAction(Close);});m.Items.Add("Exit Flow Desk",null,delegate{QueueWindowAction(Program.Context.Shutdown);});Theme.KeepMenu(anchor,m);m.Show(anchor,new Point(0,anchor.Height));}
 void ApplyLayout(){loading=true;split.Panel1Collapsed=false;split.Panel2Collapsed=!Model.Split;split.Orientation=Model.Horizontal?Orientation.Horizontal:Orientation.Vertical;split.Panel1MinSize=120;split.Panel2MinSize=120;if(Model.Split){int length=Model.Horizontal?split.Height:split.Width;int available=length-split.SplitterWidth;if(available>=240)split.SplitterDistance=Math.Max(120,Math.Min(available-120,(int)(available*Store.Ratio(Model.Ratio))));}loading=false;Sync(false);}
 public void Expand(Pane p){if(!Model.Split)return;expanded=!expanded;if(!expanded)ApplyLayout();else {split.Panel1Collapsed=p==Panes[1];split.Panel2Collapsed=p==Panes[0];}Sync(false);}
 public void Sync(bool focus){if(IsHandleCreated&&Native.Cloaked(Handle))return;foreach(var p in Panes)p.Sync(focus);}
 public void Save(){if(loading)return;Program.Context.Save();}
 ContextMenuStrip WorkspaceMenu(){var m=new ContextMenuStrip();m.Items.Add("Save workspace…",null,delegate{string name=Theme.Ask("Save workspace",Model.Name);if(!string.IsNullOrWhiteSpace(name)){Model.Name=name;Model.Saved=true;Text="Flow Desk · "+name;foreach(var panel in Panes)panel.UpdateModel();Save();}});m.Items.Add("New workspace",null,delegate{string n=Theme.Ask("Workspace name","Workspace");if(!string.IsNullOrWhiteSpace(n))Program.Context.Open(new WorkspaceModel{Name=n});});m.Items.Add("Rename workspace",null,delegate{string n=Theme.Ask("Workspace name",Model.Name);if(!string.IsNullOrWhiteSpace(n)){Model.Name=n;Text="Flow Desk · "+n;Save();}});m.Items.Add("Delete workspace…",null,delegate{if(MessageBox.Show("Delete layout and detach its windows?","Delete workspace",MessageBoxButtons.YesNo)==DialogResult.Yes){Program.Context.Session.Workspaces.Remove(Model);Close();Program.Context.Save();}});m.Items.Add(new ToolStripSeparator());foreach(var model in Program.Context.Session.Workspaces.Where(w=>w.Saved||Program.Context.Forms.Any(f=>f.Model==w)).ToArray()){var target=model;m.Items.Add(target.Name,null,delegate{var f=Program.Context.Forms.FirstOrDefault(x=>x.Model==target);if(f!=null){f.WindowState=FormWindowState.Normal;f.Activate();}else Program.Context.Open(target);});}return m;}
 public void RestoreTabs(bool quiet=false){var candidates=Candidate.All();foreach(var pane in Panes){var saved=pane.Model.Tabs.ToArray();foreach(var tab in saved){if(pane.Windows.Values.Any(w=>w.Model==tab))continue;var matches=candidates.Where(c=>string.Equals(c.Path,tab.Path,StringComparison.OrdinalIgnoreCase)&&c.Title==tab.Title).ToArray();if(matches.Length==1){try{pane.Attach(matches[0],tab);}catch(Exception ex){MessageBox.Show(ex.Message);}}else if(!quiet)MessageBox.Show("Select this saved window manually using Add window: "+tab.Name+" ("+matches.Length+" exact matches)");}}Save();}
 protected override bool ProcessCmdKey(ref Message msg,Keys key){if(key==Keys.F11){ToggleFullScreen();return true;}if(key==(Keys.Control|Keys.Tab)){var tabs=Active.Tabs;if(tabs.TabCount>0)tabs.SelectedIndex=(tabs.SelectedIndex+1)%tabs.TabCount;return true;}if(key==Keys.F6){Active=Panes[(Panes.IndexOf(Active)+1)%(Model.Split?2:1)];Active.Sync(true);return true;}return base.ProcessCmdKey(ref msg,key);}
 protected override void WndProc(ref Message m){if(m.Msg==0xa1){int hit=m.WParam.ToInt32();if(hit==2||(hit>=10&&hit<=17)){StartWindowDrag(hit);m.Result=IntPtr.Zero;return;}}if(m.Msg==0xa3&&m.WParam.ToInt32()==2){ToggleMaximize();m.Result=IntPtr.Zero;return;}if(m.Msg==0x200&&dragHit!=0){Point point=Cursor.Position;Bounds=DragBounds(dragBounds,new Point(point.X-dragStart.X,point.Y-dragStart.Y),dragHit,MinimumSize);m.Result=IntPtr.Zero;return;}if(m.Msg==0x202&&dragHit!=0){dragHit=0;Capture=false;m.Result=IntPtr.Zero;return;}if(m.Msg==0x112){int command=m.WParam.ToInt32()&0xfff0;if((command==0xf030&&WindowState!=FormWindowState.Maximized)||(command==0xf120&&WindowState==FormWindowState.Maximized)){ToggleMaximize();m.Result=IntPtr.Zero;return;}}if(m.Msg==0x84){long value=m.LParam.ToInt64();m.Result=new IntPtr(WindowHitTest(new Point((short)(value&0xffff),(short)((value>>16)&0xffff))));return;}base.WndProc(ref m);if(m.Msg==0x2e0)BeginInvoke((Action)delegate{Sync(false);});}
}
public class AppContext:ApplicationContext {
 Native.WinEventProc shellCallback;IntPtr foregroundHook,cloakHook,switchHook;bool refreshQueued;
 HashSet<IntPtr> shellReturns=new HashSet<IntPtr>();
 public int DesktopRefreshCount {get;private set;}
 void QueueDesktopRefresh(){if(exiting||refreshQueued||dispatcher==null||dispatcher.IsDisposed)return;refreshQueued=true;dispatcher.BeginInvoke((Action)delegate{refreshQueued=false;if(exiting)return;DesktopRefreshCount++;IntPtr foreground=Native.GetForegroundWindow();var returned=shellReturns.ToArray();shellReturns.Clear();foreach(var form in Forms.ToArray())if(!form.IsDisposed)form.RefreshDesktopState(foreground,returned.Contains(form.Handle));});}
 NotifyIcon tray;Control dispatcher;RegisteredWaitHandle activationWait;bool exiting;
 public Session Session;public List<Workspace> Forms=new List<Workspace>();
 public AppContext(){try{Session=Store.Read<Session>(Path.Combine(Store.Root,"session.xml"));if(Session.Workspaces.Any(w=>!w.Saved)){string backup=Path.Combine(Store.Root,"legacy-workspaces.xml");if(!File.Exists(backup))Store.Write(backup,Session);Session.Workspaces.RemoveAll(w=>!w.Saved);}}catch{Session=new Session();}if(Session.Workspaces.Count==0)Session.Workspaces.Add(new WorkspaceModel{Name="Workspace"});}
 public void Start(bool background=false){
  dispatcher=new Control();var handle=dispatcher.Handle;
  shellCallback=delegate(IntPtr hook,uint evt,IntPtr window,int obj,int child,uint thread,uint time){if(evt==0x15){foreach(var form in Forms)if(!form.IsDisposed&&form.IsHandleCreated)shellReturns.Add(form.Handle);QueueDesktopRefresh();return;}if(window==IntPtr.Zero)return;bool own=Forms.Any(f=>!f.IsDisposed&&f.IsHandleCreated&&f.Handle==window);if(own&&evt>=0x8017&&evt<=0x8018)shellReturns.Add(window);if(evt==3||own)QueueDesktopRefresh();};
  foregroundHook=Native.SetWinEventHook(3,3,IntPtr.Zero,shellCallback,0,0,0);cloakHook=Native.SetWinEventHook(0x8017,0x8018,IntPtr.Zero,shellCallback,0,0,0);switchHook=Native.SetWinEventHook(0x15,0x15,IntPtr.Zero,shellCallback,0,0,0);
  activationWait=ThreadPool.RegisterWaitForSingleObject(Program.Activation,delegate(object state,bool timeout){try{dispatcher.BeginInvoke((Action)delegate{if(!exiting)NewWorkspace(true);});}catch(InvalidOperationException){}},null,-1,false);
  var menu=new ContextMenuStrip();menu.Items.Add("New workspace / Add window",null,delegate{NewWorkspace(true);});menu.Items.Add("Exit Flow Desk",null,delegate{Shutdown();});
  tray=new NotifyIcon{Text="Flow Desk",Icon=Icon.ExtractAssociatedIcon(Application.ExecutablePath),ContextMenuStrip=menu,Visible=true};
  tray.MouseClick+=delegate(object s,MouseEventArgs e){if(e.Button==MouseButtons.Left)NewWorkspace(true);};
  if(!background){var initial=Session.StartupWorkspace();if(initial!=null)Open(initial);}
 }
 public void NewWorkspace(bool addWindow){var model=new WorkspaceModel{Name="Workspace "+(Session.Workspaces.Count+1)};var f=Open(model);var area=Screen.FromPoint(Cursor.Position).WorkingArea;f.StartPosition=FormStartPosition.Manual;f.Bounds=new Rectangle(area.X+Math.Max(0,(area.Width-f.Width)/2),area.Y+Math.Max(0,(area.Height-f.Height)/2),Math.Min(f.Width,area.Width),Math.Min(f.Height,area.Height));f.Activate();if(addWindow)f.BeginInvoke((Action)delegate{if(!f.IsDisposed)f.Active.Add();});}
 public Workspace Open(WorkspaceModel m){var existing=Forms.FirstOrDefault(f=>f.Model.Id==m.Id&&!f.IsDisposed);if(existing!=null){existing.WindowState=FormWindowState.Normal;existing.Activate();return existing;}if(!Session.Workspaces.Contains(m))Session.Workspaces.Add(m);var window=new Workspace(m);Forms.Add(window);window.Activated+=delegate{if(!exiting){Session.LastWorkspaceId=m.Id;Save();}};window.Show();Session.LastWorkspaceId=m.Id;Save();if(m.Saved)window.BeginInvoke((Action)delegate{if(!window.IsDisposed)window.RestoreTabs(true);});return window;}
 public void Save(){try{var saved=new Session{LastWorkspaceId=Session.LastWorkspaceId,Workspaces=Session.Workspaces.Where(w=>w.Saved).ToList()};Store.Write(Path.Combine(Store.Root,"session.xml"),saved);}catch(Exception ex){Debug.WriteLine(ex);}}
 public void Closed(Workspace f){Forms.Remove(f);if(!f.Model.Saved)Session.Workspaces.Remove(f.Model);Save();}
 public void Shutdown(){if(exiting)return;exiting=true;if(foregroundHook!=IntPtr.Zero)Native.UnhookWinEvent(foregroundHook);if(cloakHook!=IntPtr.Zero)Native.UnhookWinEvent(cloakHook);if(switchHook!=IntPtr.Zero)Native.UnhookWinEvent(switchHook);activationWait.Unregister(null);foreach(var f in Forms.ToArray())f.Close();Save();tray.Visible=false;tray.ContextMenuStrip.Dispose();tray.Dispose();dispatcher.Dispose();ExitThread();}
}
public static class Program {
 public static AppContext Context;
 public static EventWaitHandle Activation;
 [STAThread] public static void Main(string[] args){
  if(args.Length==2&&args[0]=="--guard"){Guard(args[1]);return;}
  if(args.Length==1&&args[0]=="--self-test"){SelfTest();return;}
  string sid=WindowsIdentity.GetCurrent().User.Value;var security=new EventWaitHandleSecurity();security.AddAccessRule(new EventWaitHandleAccessRule(new SecurityIdentifier(sid),EventWaitHandleRights.FullControl,AccessControlType.Allow));bool fresh;
  using(Activation=new EventWaitHandle(false,EventResetMode.AutoReset,"Local\\FlowDesk.Activate.v1."+sid,out fresh,security)){
   if(!fresh){if(args.Contains("--background"))return;foreach(var process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)){try{if(process.Id!=Process.GetCurrentProcess().Id&&string.Equals(process.MainModule.FileName,Application.ExecutablePath,StringComparison.OrdinalIgnoreCase))Native.AllowSetForegroundWindow((uint)process.Id);}catch{}finally{process.Dispose();}}Activation.Set();return;}
   Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);Context=new AppContext();Context.Start(args.Contains("--background"));Application.Run(Context);
  }
 }
 static void Guard(string path){try{var s=Store.Read<Snapshot>(path);var host=Process.GetProcessById(s.Host);File.WriteAllText(path+".ready","ready");while(!host.WaitForExit(400)){if(!File.Exists(path)){File.Delete(path+".ready");return;}}if(File.Exists(path)){s.Restore();File.Delete(path);}File.Delete(path+".ready");}catch(Exception ex){try{File.WriteAllText(path+".error",ex.ToString());}catch{}}}
 static void SelfTest(){var results=new List<string>();Action<bool,string> check=(ok,name)=>{if(!ok)throw new Exception(name);results.Add("PASS "+name);};check(Store.Ratio(-1)==.15,"ratio lower bound");check(Store.Ratio(2)==.85,"ratio upper bound");check(Store.Ratio(double.NaN)==.5,"invalid ratio");var s=new Session();var w=new WorkspaceModel{Name="Test",Split=true,Ratio=.35,Horizontal=true};w.Panels[0].Tabs.Add(new TabModel{Name="Custom",Path="C:\\app.exe",Title="Exact title",Mode="Aligned"});s.Workspaces.Add(w);string path=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"flowdesk-test-"+Guid.NewGuid()+".xml");try{Store.Write(path,s);var loaded=Store.Read<Session>(path);check(loaded.Workspaces[0].Panels[0].Tabs[0].Name=="Custom","tab metadata roundtrip");check(loaded.Workspaces[0].Ratio==.35&&loaded.Workspaces[0].Horizontal,"layout roundtrip");Store.Write(path,loaded);check(Store.Read<Session>(path).Workspaces.Count==1,"atomic replacement");check(!File.ReadAllText(path).Contains("Hwnd"),"session has no runtime handles");}finally{File.Delete(path);}File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"test-results.txt"),results);}
}
}
