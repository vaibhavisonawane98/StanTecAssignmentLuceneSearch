using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using MarketingCodingAssignment.Models;
using static Lucene.Net.Util.Packed.PackedInt32s;

namespace MarketingCodingAssignment.Services
{
    public class SearchEngine
    {
        //private const Lucene.Net.Util.Version _luceneVersion = Lucene.Net.Util.Version.LUCENE_30; 
        //private readonly Lucene.Net.Store.Directory _dir; // Instead of storing the lucene indexes in a local directory, we store them in a virtual directory in RAM.

        // Ensures index backward compatibility
        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        public SearchEngine()
        {
            //_dir = new RAMDirectory();
        }

        public void CreateIndexAndSchema()
        {
            return;
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

            /////////////



            //Add to the index

            var source = new
            {
                Name = "Kermit the Frog",
                FavoritePhrase = "The quick brown fox jumps over the lazy dog"
            };
                        var doc = new Document
            {
                // StringField indexes but doesn't tokenize
                new StringField("name",
                    source.Name,
                    Field.Store.YES),
                new TextField("favoritePhrase",
                    source.FavoritePhrase,
                    Field.Store.YES)
            };

            writer.AddDocument(doc);
            writer.Flush(triggerMerge: false, applyAllDeletes: false);






            // Search with a phrase
            var phrase = new MultiPhraseQuery
            {
                new Term("favoritePhrase", "brown"),
                new Term("favoritePhrase", "fox")
            };

            // Re-use the writer to get real-time updates
            using var reader = writer.GetReader(applyAllDeletes: true);
            var searcher = new IndexSearcher(reader);
            var hits = searcher.Search(phrase, 20 /* top 20 */).ScoreDocs;

            // Display the output in a table
            Console.WriteLine($"{"Score",10}" +
                $" {"Name",-15}" +
                $" {"Favorite Phrase",-40}");
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                Console.WriteLine($"{hit.Score:f8}" +
                    $" {foundDoc.Get("name"),-15}" +
                    $" {foundDoc.Get("favoritePhrase"),-40}");
            }


            //Lucene.Net.Store.Directory mydir = new RAMDirectory();

            //using (StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))

            //using (IndexWriter indexWriter = new IndexWriter(mydir, analyzer, new IndexWriter.MaxFieldLength(10000)))
            //{
            //    foreach (var film in films)
            //    {
            //        Document document = new();
            //        document.Add(new Field("Id", film.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            //        document.Add(new Field("Title", film.Title, Field.Store.YES, Field.Index.ANALYZED));
            //        document.Add(new Field("Overview", film.Overview, Field.Store.YES, Field.Index.ANALYZED));
            //        indexWriter.AddDocument(document);
            //    }
            //    indexWriter.Commit();
            //    indexWriter.Optimize();
            //    indexWriter.Flush(true, true, true);
            //}
            return;
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

        //public IEnumerable<Film> Search(string searchString)
        //{



        //    int hitCount = 0;
        //    var searchResult = new List<Film>();
        //    var analyzer = new StandardAnalyzer(_luceneVersion);
        //    var queryParser = new QueryParser(_luceneVersion, "FullText", analyzer);

        //    using (IndexReader indexReader = IndexReader.Open(_dir, true))

        //    using (IndexSearcher searcher = new IndexSearcher(indexReader))
        //    {
        //        try
        //        {
        //            Query query = queryParser.Parse(searchString);
        //            TopScoreDocCollector collector = TopScoreDocCollector.Create(hitCount, true);
        //            ScoreDoc[] match = collector.TopDocs().ScoreDocs;

        //            foreach (var item in match)
        //            {
        //                var doc = searcher.Doc(item.Doc);
        //                searchResult.Add(new Film
        //                {
        //                    Id = doc.GetField("Id").StringValue,
        //                    Title = doc.GetField("Title").StringValue,
        //                    Overview = doc.GetField("Overview").StringValue
        //                }
        //                );
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            searchResult.Clear();
        //        }
        //    }

        //    return searchResult.ToList();
        //}


    }
}
