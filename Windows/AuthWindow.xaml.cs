using System.Linq;
using System.Windows.Navigation;
using CoreTweet;
using Twitify.Resources;

namespace Twitify.Windows
{
    /// <summary>
    /// Interaction logic for AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow
    {
        private const string CallbackUri = "twitify://callback";

        public Tokens Result { get; private set; }
        private readonly OAuth.OAuthSession _session;

        public AuthWindow()
        {
            InitializeComponent();

            _session = OAuth.Authorize(
                TwitterKeys.ConsumerKey,
                TwitterKeys.ConsumerSecret,
                CallbackUri
            );

            Browser.Source = _session.AuthorizeUri;
        }

        private async void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var uri = e.Uri;
            if (!uri.ToString().StartsWith(CallbackUri)) return;
            e.Cancel = true;

            var verifier = uri.Query
                              .Split('&')
                              .First(q => q.Contains("oauth_verifier"))
                              .Split('=')[1];

            Result = await _session.GetTokensAsync(verifier);
            Close();
        }
    }
}