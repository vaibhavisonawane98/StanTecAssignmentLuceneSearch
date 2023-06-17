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
using Lucene.Net.QueryParsers.Flexible.Standard;
using Lucene.Net.QueryParsers.Classic;

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
            var indexPath = "C:\\StanTecAssignment\\coding-assignment-main\\coding-assignment-main\\LuceneIndex";
            using var dir = FSDirectory.Open(indexPath);

            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);

            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);

            //Add to the index
            foreach (var film in films)
            {
                Document document = new Document();
                document.Add(new TextField("Id", film.Id, Field.Store.YES));
                document.Add(new TextField("Title", film.Title, Field.Store.YES));
                document.Add(new TextField("Overview", film.overview, Field.Store.YES));
                document.Add(new TextField("budget", film.budget, Field.Store.YES));
                document.Add(new TextField("genres", film.genres, Field.Store.YES));
                document.Add(new TextField("original_language", film.original_language, Field.Store.YES));
                document.Add(new TextField("popularity", film.popularity, Field.Store.YES));
                document.Add(new TextField("production_companies", film.production_companies, Field.Store.YES));
                document.Add(new TextField("release_date", film.release_date, Field.Store.YES));
                document.Add(new TextField("revenue", film.revenue, Field.Store.YES));
                document.Add(new TextField("runtime", film.runtime, Field.Store.YES));
                document.Add(new TextField("tagline", film.tagline, Field.Store.YES));
                document.Add(new TextField("vote_average", film.vote_average, Field.Store.YES));
                document.Add(new TextField("vote_count", film.vote_count, Field.Store.YES));
                writer.AddDocument(document);
            }

            writer.Flush(triggerMerge: false, applyAllDeletes: false);
            writer.Commit();

           return;
        }


        public IEnumerable<Film> Search(string searchString)
        {
            // Construct a machine-independent path for the index
            var indexPath = "C:\\StanTecAssignment\\coding-assignment-main\\coding-assignment-main\\LuceneIndex";
            using var dir = FSDirectory.Open(indexPath);
            var analyzer = new StandardAnalyzer(AppLuceneVersion);

            // Create an analyzer to process the text
            var directoryReader = DirectoryReader.Open(dir);
            var searcher = new IndexSearcher(directoryReader);
            string[] fields = { "Id","Title", "Overview", "budget", "genres", "original_language", "overview",
                "popularity", "production_companies", "release_date", "revenue", "runtime", "tagline",
                "vote_average", "vote_count" };

            var queryParser = new MultiFieldQueryParser(AppLuceneVersion, fields, analyzer);

            searchString = searchString + "*";
            var query = queryParser.Parse(searchString);
            var hits = searcher.Search(query, 1000 /* top 1000 */).ScoreDocs;

            var searchResult = new List<Film>();
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);

                Film film = new Film
                {
                    Id = foundDoc.Get("Id").ToString(),
                    Title = foundDoc.Get("Title").ToString(),
                    overview = foundDoc.Get("Overview").ToString(),
                    budget= foundDoc.Get("budget").ToString(),
                    genres= foundDoc.Get("genres").ToString(),
                    original_language= foundDoc.Get("original_language").ToString(),
                    popularity = foundDoc.Get("popularity").ToString(),
                    production_companies = foundDoc.Get("production_companies").ToString(),
                    release_date = foundDoc.Get("release_date").ToString(),
                    revenue = foundDoc.Get("revenue").ToString(),
                    runtime = foundDoc.Get("runtime").ToString(),
                    tagline = foundDoc.Get("tagline").ToString(),
                    vote_average = foundDoc.Get("vote_average").ToString(),
                    vote_count = foundDoc.Get("vote_count").ToString(),
                };


                searchResult.Add(film);
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
