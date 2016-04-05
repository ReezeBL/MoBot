using MoBot.Structure.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MoBot.Structure
{
    partial class Viewer : Form, IObserver<SysAction>
    {
        internal Controller mainController;
        public Viewer()
        {
            InitializeComponent();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(SysAction value)
        {
           if(value is ActionConnect)
            {
                var connect = value as ActionConnect;
                if (connect.Connected)
                    consoleWindow.AppendText("Client connected!" + Environment.NewLine);
                else
                    consoleWindow.AppendText("Client disnnected!" + Environment.NewLine);
            }
           else if(value is ActionMessage)
            {
                var message = value as ActionMessage;
                consoleWindow.AppendText(message.message + Environment.NewLine);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            mainController.HandleConnect();
        }
    }
}
