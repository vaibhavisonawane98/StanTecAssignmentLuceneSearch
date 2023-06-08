using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucene.Net.Util;

using MarketingCodingAssignment.Models;
using System.Numerics;
using System.Reflection;
using System.IO;

namespace MarketingCodingAssignment.Services
{
    public class SearchEngine
    {
        //private const Lucene.Net.Util.Version _luceneVersion = Lucene.Net.Util.Version.LUCENE_30; 
        //private readonly Lucene.Net.Store.Directory _dir; // Instead of storing the lucene indexes in a local directory, we store them in a virtual directory in RAM.

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
            Lucene.Net.Store.Directory mydir = new RAMDirectory();
            using (var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            using (var indexWriter = new IndexWriter(mydir, analyzer, new IndexWriter.MaxFieldLength(10000)))
            {
                foreach (var film in films)
                {
                    Document document = new();
                    document.Add(new Field("Id", film.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("Title", film.Title, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Overview", film.Overview, Field.Store.YES, Field.Index.ANALYZED));
                    indexWriter.AddDocument(document);
                }
                indexWriter.Commit();
                indexWriter.Optimize();
                indexWriter.Flush(true, true, true);
            }
            return;
        }

        //public void ResetIndex()
        //{
        //    // Delete everything from the index
        //    IndexWriter indexWriter = new(_dir, new StandardAnalyzer(_luceneVersion), new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));
        //    indexWriter.DeleteAll();
        //    indexWriter.Commit();
        //    return;
        //}

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
        //                searchResult.Add(new Film {
        //                    Id = doc.GetField("Id").StringValue,
        //                    Title = doc.GetField("Title").StringValue,
        //                    Overview = doc.GetField("Overview").StringValue}
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
