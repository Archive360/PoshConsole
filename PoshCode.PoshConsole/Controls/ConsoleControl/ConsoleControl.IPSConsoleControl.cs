using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PoshCode.Controls
{
   /// <summary>
   /// Here we EXPLICITLY implement the IPSConsoleControl interface.
   /// Importantly, this implementation just calls the existing methods on the our ConsoleRichTextBox class
   /// Each call is wrapped in Dispatcher methods so that the interface is thread-safe!
   /// </summary>
   public partial class ConsoleControl // : IPoshConsoleControl  //, IPSConsole, IConsoleControlBuffered
   {
      //public event TabCompleteHandler TabComplete;
      //public event HistoryHandler GetHistory;
      public event CommmandDelegate Command;
      public event PipelineFinished Finished;
      private int _id;

      /// <summary>
      /// Right before a prompt we want to insert a new paragraph...
      /// But we want to trim any whitespace off the end of the output first 
      /// because the paragraph mark makes plenty of whitespace
      /// </summary>
      internal void OnCommandFinished(IEnumerable<Command> command, PipelineState results)
      {
         _id++;
         //// NOTE: we are only using the dispatcher, to make sure this doesn't complete before the command output
         Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)delegate
         {
            Current.Tag = _id;
            if (results != PipelineState.Completed && results != PipelineState.NotStarted)
            {
                Write(ConsoleBrushes.VerboseForeground, ConsoleBrushes.VerboseBackground, $"VERBOSE: PowerShell Pipeline was: {results}\n");
            }
             Finished?.Invoke(this, new FinishedEventArgs { Commands = command, Results = results });
         });
      }

      public RichTextBox CommandBox
      {
         get
         {
            return _commandBox;
         }
      }

      // Using a DependencyProperty as the backing store for CaretColor.  This enables animation, styling, binding, etc...
      public static readonly DependencyProperty CaretColorProperty =
          DependencyProperty.Register("CaretColor", typeof(Color), typeof(ConsoleControl), new UIPropertyMetadata(Colors.White));

      public Color CaretColor
      {
         get { return (Color)GetValue(CaretColorProperty); }
         set { SetValue(CaretColorProperty, value); }
      }

      //, CaretColorChanged
      //public static void CaretColorChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
      //{
      //   var console = source as ConsoleControl;
      //   if (console != null)
      //   {
      //      Color c = (Color)args.NewValue;
      //      console._commandBox.Background = new SolidColorBrush();
      //   }
      //}

      //public Runspace Runspace
      //{
      //   get; set;
      //}
   }
}