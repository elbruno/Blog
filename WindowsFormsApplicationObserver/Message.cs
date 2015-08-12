namespace WindowsFormsApplicationObserver
{
    public class Message 
    {
        private string _source;
        private string _information;

        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public string Information
        {
            get { return _information; }
            set { _information = value; }
        }
    }
}
