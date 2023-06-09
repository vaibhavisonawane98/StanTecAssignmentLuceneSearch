using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Diagnostics;
using System.IO;
using System.Text;
using MarketingCodingAssignment.Models;
using static Lucene.Net.Util.Packed.PackedInt32s;

namespace MarketingCodingAssignment.Services
{
    public class SearchEngine
    {
        // The code below is roughly based on sample code from: https://lucenenet.apache.org/

        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        public SearchEngine()
        {

        }

        public void PopulateIndex(List<Film> films)
        {
            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");
            using var dir = FSDirectory.Open(indexPath);

            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);

            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);

            //Add to the index
            foreach (var film in films)
            {
                Document doc = new Document
                {
                    new StringField("Id", film.Id, Field.Store.YES),
                    new TextField("Title", film.Title, Field.Store.YES),
                    new TextField("Overview", film.Overview, Field.Store.YES)
                };
                writer.AddDocument(doc);
            }

            writer.Flush(triggerMerge: false, applyAllDeletes: false);
            writer.Commit();

           return;
        }


        public IEnumerable<Film> Search(string searchString)
        {
            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");
            using var dir = FSDirectory.Open(indexPath);

            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);

            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);

            // Search with a phrase
            var phrase = new PhraseQuery
            {
                new Term("Overview", searchString)
            };

            // Re-use the writer to get real-time updates
            using var reader = writer.GetReader(applyAllDeletes: true);
            var searcher = new IndexSearcher(reader);
            var hits = searcher.Search(phrase, 25).ScoreDocs;

            var searchResult = new List<Film>();
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);

                // return a list of films
                Film film = new Film
                {
                    Id = foundDoc.GetField("Id").ToString(),
                    Title = foundDoc.GetField("Title").ToString(),
                    Overview = foundDoc.GetField("Overview").ToString()
                };
                searchResult.Add(film);

                // Display the output in a table in the VS Output
                Console.WriteLine($"{"Score",10}" +
                    $" {"Id",-15}" +
                    $" {"Title",-25}" +
                    $" {"Overview",-40}");

                Debug.WriteLine($"{hit.Score:f8}" +
                    $" {film.Id,-15}" +
                    $" {film.Title,-25}" +
                    $" {film.Overview,-40}");

            }

            return searchResult.ToList();
        }

        public void DeleteIndexContents()
        {
            // Delete everything from the index
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");
            using var dir = FSDirectory.Open(indexPath);
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);
            writer.DeleteAll();
            writer.Commit();
            return;
        }

    }
}
