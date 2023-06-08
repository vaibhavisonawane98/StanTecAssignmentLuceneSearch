using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.Diagnostics;

using MarketingCodingAssignment.Models;
using System.Numerics;
using System.Reflection;

namespace MarketingCodingAssignment.Services
{
    public class SearchEngine
    {
        private const Lucene.Net.Util.Version _luceneVersion = Lucene.Net.Util.Version.LUCENE_30;

        // This is the location where the Lucene index files are created / stored.
        private const String indexPath = "/solr/solr/Collection1/data/index"

        public SearchEngine()
        {
        }

        public void CreateIndexAndSchema()
        {
            return;
        }

        public void PopulateIndex(List<Film> films)
        {
            //File path = new File(" ... /solr/solr/Collection1/data/index");
            var directory = new RAMDirectory();
            var analyzer = new StandardAnalyzer(_luceneVersion);
            var indexWriter = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH));

            foreach (var film in films)
            {
                Document document = new Document();
                document.Add(new Field("Id", film.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("Title", film.Title, Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("Overview", film.Overview, Field.Store.YES, Field.Index.NOT_ANALYZED));
                indexWriter.AddDocument(document);
            }

            indexWriter.Commit();
            indexWriter.Optimize();
            indexWriter.Flush(true, true, true);

            return;

        }





        public IEnumerable<Film> Search(string searchString)
        {
            int hitCount = 0;
            var searchResult = new List<Film>();

            var dir = new RAMDirectory();
            var analyzer = new StandardAnalyzer(_luceneVersion);
            var queryParser = new QueryParser(_luceneVersion, "FullText", analyzer);

            using (IndexReader indexReader = IndexReader.Open(indexDirectory, true))
            //using (DirectoryReader directoryReader = wri)

            using (var searcher = new IndexSearcher(indexReader))
            {
                try
                { 
                    var query = queryParser.Parse(searchString);

                    var collector = TopScoreDocCollector.Create(hitCount, true);

                    searcher.Search(query, collector);

                    var match = collector.TopDocs().ScoreDocs;

                    foreach (var item in match)
                    {
                        var id = item.Doc;
                        var doc = searcher.Doc(id);

                        searchResult.Add(new Film {
                            Id = Int32.Parse(doc.GetField("Id").StringValue),
                            Title = doc.GetField("Title").StringValue,
                            Overview = doc.GetField("Overview").StringValue}
                        ); 
                    }
                }
                catch (Exception)
                {
                    searchResult.Clear();
                }

            }

            return searchResult;
        }


        public static SearchIndexSearcher Create(Organization organization, SearchConfig searchConfig)
        {
            var indexFolder = Path.Combine(searchConfig.IndexRootFolder, $"lucene_index_{organization.Id}");
            var reader = DirectoryReader.Open(FSDirectory.Open(indexFolder));
            return new SearchIndexSearcher(indexFolder, reader);
        }

        public async Task<IList<Asset>> SearchAssets(string searchPhrase)
        {
            var searcher = new IndexSearcher(_indexReader);
            var queryParser = new QueryParser(_luceneVersion, "Content",
                      new StandardAnalyzer(_luceneVersion));
            var query = queryParser.Parse(searchPhrase);
            var hits = searcher.Search(query, _indexReader.MaxDoc).ScoreDocs;

            return hits.Select(hit => searcher.Doc(hit.Doc)).Select(doc => new Asset
            { Id = Convert.ToInt32(doc.Get("AssetId")), Name = doc.Get("Name") }).ToList();
        }




    }
}
