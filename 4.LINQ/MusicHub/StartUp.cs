namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here

            Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var queryResult = context.Albums
                .Where(p => p.ProducerId == producerId)
                .Select(x => new
                {
                    x.Name,
                    x.ReleaseDate,
                    x.Producer,
                    x.Price,
                    x.Songs
                })
                .ToList()
                .OrderByDescending(x => x.Price);

            StringBuilder sb = new StringBuilder();

            foreach (var album in queryResult)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}");
                sb.AppendLine($"-ProducerName: {album.Producer.Name}");
                sb.AppendLine($"-Songs:");

                int songCount = 1;

                foreach (var song in album.
                    Songs.
                    OrderByDescending(x => x.Name)
                    .ThenBy(x => x.Writer.Name))
                {
                    sb.AppendLine($"---#{songCount}");
                    sb.AppendLine($"---SongName: {song.Name}");
                    sb.AppendLine($"---Price: {song.Price:F2}");
                    sb.AppendLine($"---Writer: {song.Writer.Name}");

                    songCount++;
                }

                sb.AppendLine($"-AlbumPrice: {album.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            TimeSpan durationSpan = new TimeSpan(0, 0, duration);

            var songsAboveDuration = context
                .Songs
                .Where(x => x.Duration > durationSpan)
                .Include(x => x.SongPerformers)
                .ThenInclude(x => x.Performer)
                .Select(x => new
                {
                    SongName = x.Name,
                    Performers = x.SongPerformers,
                    WriterName = x.Writer.Name,
                    ProducerName = x.Album.Producer.Name,
                    Duration = x.Duration
                })
                .ToList()
                .OrderBy(x => x.SongName)
                .ThenBy(x => x.WriterName)
                .ThenBy(x => x.Performers.Select(x => x.Performer.FirstName + " " + x.Performer.LastName));

            StringBuilder sb = new StringBuilder();

            int songCounter = 1;

            foreach (var song in songsAboveDuration)
            {
                sb.AppendLine($"-Song #{songCounter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                string performer = "";

                if (song.Performers.FirstOrDefault() != null)
                {
                    var realPerformer = song.Performers.First().Performer;

                    performer = $"{realPerformer.FirstName} {realPerformer.LastName}";
                }

                sb.AppendLine($"---Performer: {performer}");

                sb.AppendLine($"---AlbumProducer: {song.ProducerName}");
                sb.AppendLine($"---Duration: {song.Duration.ToString("c")}");

                songCounter++;
            }

            return sb.ToString().TrimEnd();
        }
    }
}
