using CoreTweet;
using SpotifyAPI.Local;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
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

        private readonly TimeSpan _animationDuration = TimeSpan.FromMilliseconds(500);
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
            Dispatcher.Invoke(() =>
            {
                ShowWindow();
                ChangeStatus(false);
            });

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
            Dispatcher.Invoke(() =>
            {
                ChangeStatus(true);
                HideWindowAsync(3000);
            });
        }

        private void ChangeStatus(bool isSuccess)
        {
            var showAnimation = new DoubleAnimation(1, _animationDuration);
            var hideAnimation = new DoubleAnimation(0, _animationDuration);
            
            if (isSuccess)
            {
                StatusBusy.BeginAnimation(OpacityProperty, hideAnimation);
                StatusSuccess.BeginAnimation(OpacityProperty, showAnimation);
            }
            else
            {
                StatusBusy.BeginAnimation(OpacityProperty, showAnimation);
                StatusSuccess.BeginAnimation(OpacityProperty, hideAnimation);
            }
        }

        private void ShowWindow()
        {
            ChangeWindowOpacity(1);
        }

        private async void HideWindowAsync(int delay = 0)
        {
            if (delay != 0) await Task.Delay(delay);
            ChangeWindowOpacity(0);
        }

        private void ChangeWindowOpacity(double opacity)
        {
            BeginAnimation(
                OpacityProperty,
                new DoubleAnimation(opacity, _animationDuration)
            );
        }
    }
}