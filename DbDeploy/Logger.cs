using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace OmniDbDeploy
{
    public sealed class Logger
    {
        static readonly Logger instance = new Logger();

        public enum LogLevel
        {
            info,
            progress,
            change,
            warning,
            error,
            ddl,
            ddlChange
        }

        private RichTextBox fullLogText;
        private RichTextBox changeLogText;
        private RichTextBox warningLogText;
        private RichTextBox errorLogText;
        private RichTextBox ddlLogText;

        private TabPage changeTab;
        private TabPage warningTab;
        private TabPage errorTab;
      
        private StatusStrip statusFooter;
        private ToolStripLabel phaseLabel;
        private ToolStripLabel statusLabel;
        private ToolStripProgressBar[] progressBar = new ToolStripProgressBar[2];
        private string indentPrefix = "";

        private int changeCount = 0;
        private int warningCount = 0;
        private int errorCount = 0;

        private const bool logStatusItems = false;

        static Logger()
        {
        }

        Logger()
        {
        }

        public static Logger Singleton
        {
            get
            {
                return instance;
            }
        }

        public static Logger GetSingleton()
        {
            return instance;
        }

        public int warningsIssued
        {
            get
            {
                return warningCount;
            }
        }

        public int errorsDetected
        {
            get
            {
                return errorCount;
            }
        }

        public int changesMade
        {
            get
            {
                return changeCount;
            }
        }

        public void indent()
        {
            indentPrefix += "    ";
        }

        public void unindent()
        {
            indentPrefix = indentPrefix.Substring(4);
        }

        delegate void countsResetDisplayDelegate();

        private void countsResetDisplay()
        {
            changeTab.Text = "Change Log";
            warningTab.Text = "Warning Log";
            errorTab.Text = "Error Log";
        }

        public void countsReset()
        {
            changeCount = 0;
            warningCount = 0;
            errorCount = 0;

            if (changeTab.InvokeRequired)
            {
                countsResetDisplayDelegate d = new countsResetDisplayDelegate(countsResetDisplay);
                changeTab.Invoke(d, new object[] { });
            }
            else
            {
                countsResetDisplay();
            }
        }

        delegate void fullLogDelegate(Color color, string text);

        public void fullLog(Color color, string text)
        {
            if (fullLogText.InvokeRequired)
            {
                fullLogDelegate d = new fullLogDelegate(fullLog);
                fullLogText.Invoke(d, new object[] { color, text });
            }
            else
            {
                fullLogText.SelectionColor = color;
                fullLogText.AppendText(indentPrefix + text + "\r\n");
                fullLogText.ScrollToCaret();
            }
        }

        delegate void changeLogDelegate(Color color, string text);

        public void changeLog(Color color, string text)
        {
            if (changeLogText.InvokeRequired)
            {
                changeLogDelegate d = new changeLogDelegate(changeLog);
                changeLogText.Invoke(d, new object[] { color, text });
            }
            else
            {
                changeTab.Text = "Change Log (" + ++changeCount + ")";
                changeLogText.SelectionColor = color;
                changeLogText.AppendText(changeCount + ":\t" + text + "\r\n");
                changeLogText.ScrollToCaret();
            }
        }

        delegate void warningLogDelegate(Color color, string text);

        public void warningLog(Color color, string text)
        {
            if (warningLogText.InvokeRequired)
            {
                warningLogDelegate d = new warningLogDelegate(warningLog);
                warningLogText.Invoke(d, new object[] { color, text });
            }
            else
            {
                warningTab.Text = "Warning Log (" + ++warningCount + ")";
                warningLogText.SelectionColor = color;
                warningLogText.AppendText(warningCount + ":\t" + text + "\r\n");
                warningLogText.ScrollToCaret();
            }
        }

        delegate void errorLogDelegate(Color color, string text);

        public void errorLog(Color color, string text)
        {
            if (errorLogText.InvokeRequired)
            {
                errorLogDelegate d = new errorLogDelegate(errorLog);
                errorLogText.Invoke(d, new object[] { color, text });
            }
            else
            {
                errorTab.Text = "Error Log (" + ++errorCount + ")";
                errorLogText.SelectionColor = color;
                errorLogText.AppendText(errorCount + ":\t" + text + "\r\n");
                errorLogText.ScrollToCaret();
            }
        }

        delegate void ddlLogDelegate(Color color, string text);

        public void ddlLog(Color color, string text)
        {
            if (ddlLogText.InvokeRequired)
            {
                ddlLogDelegate d = new ddlLogDelegate(ddlLog);
                ddlLogText.Invoke(d, new object[] { color, text });
            }
            else
            {
                ddlLogText.SelectionColor = color;
                ddlLogText.AppendText(text + "\r\n");
                ddlLogText.ScrollToCaret();
            }
        }

        public void log(LogLevel logLevel, string text)
        {
            Color color = Color.Black;

            switch (logLevel)
            {
                case LogLevel.change:
                    color = Color.DarkGreen;
                    changeLog(color, text);
                    break;

                case LogLevel.ddl:
                    ddlLog(Color.Gray, text);
                    break;

                case LogLevel.ddlChange:
                    ddlLog(Color.DarkGreen, text);
                    break;

                case LogLevel.error:
                    color = Color.Red;
                    errorLog(color, text);
                    break;

                case LogLevel.info:
                    color = Color.Gray;
                    break;

                case LogLevel.progress:
                    color = Color.Black;
                    break;

                case LogLevel.warning:
                    color = Color.Blue;
                    warningLog(color, text);
                    break;
            }

            if (logLevel != LogLevel.ddl && logLevel != LogLevel.ddlChange)
                fullLog(color, text);

        }

        delegate void progressHideDelegate(int bar);

        public void progressHide(int bar)
        {
            if (statusFooter.InvokeRequired)
            {
                progressHideDelegate d = new progressHideDelegate(progressHide);
                statusFooter.Invoke(d, new object[]{ bar });
            }
            else
                progressBar[bar].Visible = false;
            //statusFooter.Refresh();
        }

        public void progressSet(int bar)
        {
            progressSet(bar, 100);
        }

        delegate void progressSetDelegate(int bar, int maximum);

        public void progressSet(int bar, int maximum)
        {
            if (statusFooter.InvokeRequired)
            {
                progressSetDelegate d = new progressSetDelegate(progressSet);
                statusFooter.Invoke(d, new object[]{ bar, maximum });
            }
            else
            {
                progressBar[bar].Value = 0;
                progressBar[bar].Maximum = maximum;
                progressBar[bar].Visible = true;
            }
            //statusFooter.Refresh();
        }

        delegate void progressUpdateDelegate(int bar, int value);

        public void progressUpdate(int bar, int value)
        {
            if( statusFooter.InvokeRequired )
            {
                progressUpdateDelegate d = new progressUpdateDelegate(progressUpdate);
                statusFooter.Invoke(d, new object[]{ bar,value });
            }
            else
                progressBar[bar].Value = value;
            //statusFooter.Refresh();
        }

        delegate void progressIncrementDelegate(int bar);

        public void progressIncrement(int bar)
        {
            if (statusFooter.InvokeRequired)
            {
                progressIncrementDelegate d = new progressIncrementDelegate(progressIncrement);
                statusFooter.Invoke(d, new object[]{ bar });
            }
            else
                progressBar[bar].Value++;
            //statusFooter.Refresh();
        }

        /*
        public int progressValue()
        {
            return progressBar.Value;
        }
        */

        delegate void phaseUpdateDelegate(string phase);

        public void phaseUpdate(string phase)
        {
            if (statusFooter.InvokeRequired)
            {
                phaseUpdateDelegate d = new phaseUpdateDelegate(phaseUpdate);
                statusFooter.Invoke(d, new object[] { phase });
            }
            else
            {
                this.phaseLabel.Text = phase;
                //this.statusFooter.Refresh();

                if (logStatusItems && phase != "")
                    log(Logger.LogLevel.progress, "PHASE: " + phase);
            }
        }

        delegate void statusUpdateDelegate(string status);

        public void statusUpdate(string status)
        {
            if (statusFooter.InvokeRequired)
            {
                statusUpdateDelegate d = new statusUpdateDelegate(statusUpdate);
                statusFooter.Invoke(d, new object[] { status });
            }
            else
            {
                this.statusLabel.Text = status;
                //this.statusFooter.Refresh();

                if (logStatusItems && status != "")
                    log(Logger.LogLevel.progress, "STATUS: " + status);
            }
        }

        public void initialize(
            RichTextBox fullLogText,
            RichTextBox changeLogText,
            RichTextBox warningLogText,
            RichTextBox errorLogText,
            RichTextBox ddlLogText,
            TabPage changeTab,
            TabPage warningTab,
            TabPage errorTab,
            StatusStrip statusFooter,
            ToolStripLabel phaseLabel,
            ToolStripLabel statusLabel,
            ToolStripProgressBar tableProgressBar,
            ToolStripProgressBar rowProgressBar
            )
        {
            this.fullLogText = fullLogText;
            this.changeLogText = changeLogText;
            this.warningLogText = warningLogText;
            this.errorLogText = errorLogText;
            this.ddlLogText = ddlLogText;
            this.changeTab = changeTab;
            this.warningTab = warningTab;
            this.errorTab = errorTab;
            this.statusFooter = statusFooter;
            this.phaseLabel = phaseLabel;
            this.statusLabel = statusLabel;

            this.progressBar[0] = tableProgressBar;
            this.progressBar[1] = rowProgressBar;



            changeLogText.SelectionHangingIndent = 30;
            changeLogText.SelectionTabs = new int[]{30};

            warningLogText.SelectionHangingIndent = 30;
            warningLogText.SelectionTabs = new int[] { 30 };

            errorLogText.SelectionHangingIndent = 30;
            errorLogText.SelectionTabs = new int[] { 30 };
        }
    }
}
