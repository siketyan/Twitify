using SpotifyAPI.Local;
using System.Windows;

namespace Twitify.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private SpotifyLocalAPI _spotify;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialize(object sender, RoutedEventArgs e)
        {
            _spotify = new SpotifyLocalAPI
            {
                ListenForEvents = true
            };

            _spotify.OnTrackChange += OnPlayingTrackChanged;
            _spotify.Connect();
        }

        private void OnPlayingTrackChanged(object sender, TrackChangeEventArgs e)
        {
            var track = e.NewTrack;
            Dispatcher.Invoke(() =>
            {
                Title.Content = track.TrackResource.Name;
                Artist.Content = track.ArtistResource.Name;
                Album.Content = track.AlbumResource.Name;
            });
        }
    }
}