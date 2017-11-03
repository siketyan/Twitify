using CoreTweet;
using SpotifyAPI.Local;
using System.Windows;
using Twitify.Objects;
using Twitify.Resources;
using Twitify.Utilities;

namespace Twitify.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string CredentialsPath = "credentials.json";

        private Credentials _credentials;
        private Tokens _tokens;
        private SpotifyLocalAPI _spotify;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            _credentials = Credentials.Open(CredentialsPath);

            if (_credentials.AccessToken.IsNullOrEmpty() ||
                _credentials.AccessTokenSecret.IsNullOrEmpty())
            {
                var authWindow = new AuthWindow();
                authWindow.ShowDialog();

                var result = authWindow.Result;
                _credentials.AccessToken = result.AccessToken;
                _credentials.AccessTokenSecret = result.AccessTokenSecret;
                _credentials.Save();
            }

            _tokens = Tokens.Create(
                TwitterKeys.ConsumerKey,
                TwitterKeys.ConsumerSecret,
                _credentials.AccessToken,
                _credentials.AccessTokenSecret
            );

            _spotify = new SpotifyLocalAPI
            {
                ListenForEvents = true
            };

            _spotify.OnTrackChange += OnPlayingTrackChangedAsync;
            _spotify.Connect();
        }

        private async void OnPlayingTrackChangedAsync(object sender, TrackChangeEventArgs e)
        {
            var track = e.NewTrack;
            if (track == null) return;

            var title = track.TrackResource.Name;
            var artist = track.ArtistResource.Name;
            var album = track.AlbumResource.Name;

            Dispatcher.Invoke(() =>
            {
                Title.Content = title;
                Artist.Content = artist;
                Album.Content = album;
            });

            await _tokens.Statuses.UpdateAsync($"NowPlaying: {title} - {artist} ({album})");
        }
    }
}