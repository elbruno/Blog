using System;
using System.Windows.Forms;
using SimpleBroker;

namespace WindowsFormsApplicationObserver
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            Load += FormMain_Load;
            Closed += FormMain_Closed;
        }

        private void FormMain_Closed(object sender, EventArgs e) => this.Unsubscribe<Message>();

        private void FormMain_Load(object sender, EventArgs e) => this.Subscribe<Message>(OnNext);

        private void btnOpenMessageDispatcher_Click(object sender, EventArgs e) => new FormSendData().Show(this);

        public void OnNext(Message value) => txtLog.Text = $"{value.Source}, {value.Information}{Environment.NewLine}{txtLog.Text}";
    }
}
