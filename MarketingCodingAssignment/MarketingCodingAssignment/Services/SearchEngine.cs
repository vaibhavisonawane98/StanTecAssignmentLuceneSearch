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

namespace MarketingCodingAssignment.Services
{
    public class SearchEngine
    {
        // The code below is roughly based on sample code from: https://lucenenet.apache.org/

        private const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

        public SearchEngine()
        {
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

            ////////////////////////////////////////////////////////////////////



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

            ////////////////////////////////////////////////////////////////////


            // Search with a phrase
            var phrase = new PhraseQuery
            {
                new Term("Overview", "test")
            };

            // Re-use the writer to get real-time updates
            using var reader = writer.GetReader(applyAllDeletes: true);
            var searcher = new IndexSearcher(reader);
            var hits = searcher.Search(phrase, 25).ScoreDocs;

            // Display the output in a table
            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                Debug.WriteLine($"{hit.Score:f8}" +
                    $" {foundDoc.Get("Id"),-15}" +
                    $" {foundDoc.Get("Title"),-40}" + 
                    $" {foundDoc.Get("Overview"),-40}");


            }


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
