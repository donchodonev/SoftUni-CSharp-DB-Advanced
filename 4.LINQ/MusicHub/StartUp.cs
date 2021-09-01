namespace MusicHub
{
    using System;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here
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
            throw new NotImplementedException();
        }
    }
}
