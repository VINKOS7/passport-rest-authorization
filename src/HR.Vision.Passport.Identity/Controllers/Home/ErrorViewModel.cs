namespace E.Shop.Passport.Identity
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage Error { get; set; }
    }

    public class ErrorMessage
    {
        public string DisplayMode
        {
            get;
            set;
        }

        public string UiLocales
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }

        public string ErrorDescription
        {
            get;
            set;
        }

        public string RequestId
        {
            get;
            set;
        }

        public string RedirectUri
        {
            get;
            set;
        }

        public string ResponseMode
        {
            get;
            set;
        }

        public string ClientId
        {
            get;
            set;
        }
    }
}