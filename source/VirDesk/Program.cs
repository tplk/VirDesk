using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using WindowsDesktop;
using Mono.Options;

namespace VirDesk
{
  static class Program
  {
    static int desktopNumber = 0; //number of Virtual Desktop to be used
    static string command;       //command to be executed
    static string arguments;     //arguments to be used with command

    static void Main(string[] args)
    {
      //if (args.Length == 0)
      //{
      //  return; //need at least 1 arg.
      //}
      run();
      return;
    }

    static void run()
    {
      //String[] clArgs = parseCommandLine(Environment.CommandLine);

      //Test input
      desktopNumber = 2;
      command = "notepad";
      arguments = "C:/Users/tplk/Documents/lightscreen_config.ini";

      Process proc = new Process();

      int index = desktopNumber - 1; //set desktop index
      proc.StartInfo.FileName = command; //set executable name
      proc.StartInfo.Arguments = arguments; //set arg list

      //If we're opening a program on desktop 10, ensure there are 10 desktops.
      for (int i = VirtualDesktop.GetCount() - 1; i < index; i++)
      {
        VirtualDesktop.Create();
      }

      //get current desktop
      VirtualDesktop currentDesk = VirtualDesktop.Current;
      //get the desktop, or create a new one
      VirtualDesktop desk = index < 0 ? VirtualDesktop.Create() : VirtualDesktop.GetDesktops()[index];

      if (!command.Equals(""))
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
  }
}
