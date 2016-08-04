using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WindowsDesktop;

namespace VirDesk
{
  static class Program
  {
    static readonly int executable = 0;
    static readonly int desktopIndex = 1;
    static readonly int command = 2;
    static readonly int arguments = 3;

    static void Main(string[] args)
    {
      run();
      return;
    }

    static void run()
    {
      String[] clArgs = parseCommandLine(Environment.CommandLine);

      //Test input
      clArgs[executable] = "1";
      clArgs[desktopIndex] = "3";
      clArgs[command] = "notepad";
      clArgs[arguments] = "C:/Users/tplk/Documents/lightscreen_config.ini";

      Process proc = new Process();

      int index = int.Parse(clArgs[desktopIndex]) - 1; //set desktop index
      proc.StartInfo.FileName = clArgs[command]; //set executable name
      proc.StartInfo.Arguments = clArgs[arguments]; //set arg list

      //If we're opening a program on desktop 10, ensure there are 10 desktops.
      for (int i = VirtualDesktop.GetCount() - 1; i < index; i++)
      {
        VirtualDesktop.Create();
      }

      //get current desktop
      VirtualDesktop currentDesk = VirtualDesktop.Current;
      //get the desktop, or create a new one
      VirtualDesktop desk = index < 0 ? VirtualDesktop.Create() : VirtualDesktop.GetDesktops()[index];

      if (!clArgs[executable].Equals(""))
      { //if we're launching a program:
        try
        {
          //try starting the program
          proc.Start();
          proc.WaitForInputIdle();

          IntPtr handle = proc.MainWindowHandle;
          handle.MoveToDesktop(desk);
          //desktop.Switch();

          //currentDesk.MakeVisible();

        }
        catch (Exception)
        {
          //Error launching program.
          Console.Error.WriteLine("Failed to start program.\nCheck executable path.");

        }
        finally
        {
          //If we created a desktop just for this program, remove it after the program has finished executing.
          if (index < 0)
          {
            proc.WaitForExit();
            desk.Remove();
          }

        }
      }

      return;
    }

    static String[] parseCommandLine(String cla)
    {
      String[] ret = new string[4];

      GroupCollection groups = Regex.Match(cla, @"(\""[^\""]+\""|[\w-:_\/\.\\]+) +(?:(-?\d+) ?)?(\""[^\""]+\""|[\w-:_\/.\\]+)? ?(.*)").Groups;

      for (int i = 1; i < 4; i++)
        ret[i - 1] = groups[i].Value; //set return values

      if (ret[desktopIndex].Equals(""))
        ret[desktopIndex] = "0";  //if index is empty, set index = 0

      return ret;
    }
  }
}
