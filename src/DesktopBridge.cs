using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
namespace FlowDesk {
public static class DesktopBridge {
 static string Run(string arguments,out string error){error="";string worker=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"FlowDesk.DesktopWorker.exe");if(!File.Exists(worker)){error="Local desktop helper is missing.";return null;}try{using(var process=Process.Start(new ProcessStartInfo(worker,arguments){UseShellExecute=false,CreateNoWindow=true,RedirectStandardOutput=true,RedirectStandardError=true})){var line=Task.Factory.StartNew(()=>process.StandardOutput.ReadLine());if(!line.Wait(6000)){try{process.Kill();}catch{}error="Desktop helper timed out; no confirmed move.";return null;}string result=line.Result; // The upstream DLL can remain in COM shutdown after returning its response.
  if(!process.WaitForExit(250))try{process.Kill();process.WaitForExit(1000);}catch{}
  if(result==null||!result.StartsWith("OK|",StringComparison.Ordinal)){error=result??"Desktop helper did not return a result.";return null;}return result;
 }}catch(Exception e){error=e.Message;return null;}}
 public static bool Probe(out int count,out int current,out string error){count=0;current=-1;string result=Run("--probe",out error);if(result==null)return false;var fields=result.Split('|');return fields.Length==3&&int.TryParse(fields[1],out count)&&int.TryParse(fields[2],out current)&&count>0&&current>=0&&current<count;}
 static string Identity(IntPtr window){int pid=(int)Native.Pid(window);long ticks=Process.GetProcessById(pid).StartTime.ToUniversalTime().Ticks;return window.ToInt64().ToString(CultureInfo.InvariantCulture)+" "+pid+" "+ticks;}
 public static bool MoveHere(IntPtr window,IntPtr host,out int original,out string error){string ignored;return MoveHere(window,host,out original,out ignored,out error);}
 public static bool MoveHere(IntPtr window,IntPtr host,out int original,out string originalId,out string error){original=-1;originalId="";try{string result=Run("--move "+Identity(window)+" "+Identity(host),out error);if(result==null)return false;var fields=result.Split('|');Guid id;if(fields.Length!=4||!int.TryParse(fields[1],out original)||!Guid.TryParse(fields[3],out id))return false;originalId=id.ToString("D");return true;}catch(Exception e){error=e.Message;return false;}}
 public static bool Restore(IntPtr window,int pid,long ticks,int desktop,out string error,string originalId=null){error="";if(desktop<0)return true;return Run("--move "+window.ToInt64()+" "+pid+" "+ticks+" desktop "+desktop+" "+(string.IsNullOrEmpty(originalId)?"0":originalId),out error)!=null;}
 public static void RestoreCandidate(Candidate candidate){if(candidate.OriginalDesktop<0)return;try{if(candidate.ProcessId==0||candidate.Started==0)return;string error;Restore(candidate.H,candidate.ProcessId,candidate.Started,candidate.OriginalDesktop,out error,candidate.OriginalDesktopId);}catch{}}
}
}
