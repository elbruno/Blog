using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApplicationObserver
{
    public partial class FormMain : Form, IObserver<Message>
    {

        public FormMain()
        {
            InitializeComponent();
        }
       

        private void btnOpenMessageDispatcher_Click(object sender, System.EventArgs e)
        {
            var formSendData = new FormSendData();
            formSendData.Show(this);
        }

        public void OnNext(Message value)
        {
            txtLog.Text = $"{value.Source}, {value.Information}{Environment.NewLine}{txtLog.Text}";
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        private void NewAction(Message value)
        {
            txtLog.Text = $"{value.Source}, {value.Information}{Environment.NewLine}{txtLog.Text}";
        }
    }
}
