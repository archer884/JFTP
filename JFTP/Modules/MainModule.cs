using JFTP.Lib;
using Nancy;
using Nancy.ModelBinding;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace JFTP.Modules
{
    public class MainModule : NancyModule
    {
        #region internals
        private static readonly string BaseDirectory = ConfigurationManager.AppSettings["BaseDirectory"];
        /*
         * TODO:
         * We'll come up with a more robust configuration mechanism at a later date.
         * */

        private static Logger _log { get; set; }
        private JDirectory _directoryStructure { get; set; }
        private IDictionary<string, string> _library { get; set; }
        #endregion

        #region constructors
        public MainModule()
        {
            var initializationTime = Util.TimeExecution(() => 
            {
                _log = LogManager.GetCurrentClassLogger();
                _directoryStructure = new JDirectory(BaseDirectory);
                _library = _directoryStructure.AsLibrary();
            });
            _log.Info("Initialization complete in: {0}", initializationTime);

            // GET: /dir or /ls
            Get["/dir"] = Get["/ls"] = _ => GetDirectoryListing();

            // GET: /library
            Get["/library"] = _ => GetLibrary();

            // GET: /files/whole/file/path
            Get["/files/{Path*}"] = _ => GetFile(_.Path);

            // GET: /token/detail/wefas23
            Get["/token/detail/{Token}"] = _ => GetFileDetailByToken(_.Token);

            // GET: /token/detail/wefas23/10
            Get["/token/detail/{Token}/{Chunks}"] = _ => GetFileDetailByToken(_.Token, _.Chunks);

            // GET: /token/25/13
            Get["/token/{Token}/{Chunks}/{ChunkNumber}"] = _ => GetFileByChunk(this.Bind<ChunkRequest>());
        }
	    #endregion

        #region methods
        public dynamic GetDirectoryListing()
        {
            _log.Info("serving /dir");
            return Negotiate
                .WithStatusCode(HttpStatusCode.OK)
                .WithModel(_directoryStructure)
                .WithView("Dir");
        }

        public dynamic GetLibrary()
        {
            _log.Info("serving /library");
            return Negotiate
                .WithStatusCode(HttpStatusCode.OK)
                .WithModel(_library);
        }

        public dynamic GetFile(string path)
        {
            var fullPath = Path.Combine(BaseDirectory, path);

            _log.Info("received request for " + fullPath);

            if (File.Exists(fullPath))
            {
                return new Response()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = ContentType.File,
                    Contents = stream =>
                    {
                        var fileStream = new FileStream(
                            fullPath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read);
                        fileStream.CopyTo(stream);
                        _log.Info("transfer complete for " + fullPath);
                    }
                };
            }
            else
            {
                _log.Info(fullPath + " not found");
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }
        }

        public dynamic GetFileDetailByToken(string token)
        {
            _log.Info("request for " + token);

            if (_library.ContainsKey(token))
            {
                _log.Info("returned detail for " + token);
                return Negotiate
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithModel(new JFileDetail(_library[token]));
            }
            else
            {
                _log.Info(token + " not found");
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }
        }

        public dynamic GetFileDetailByToken(string token, int chunks)
        {
            _log.Info(string.Format("request for {0} in {1} chunks", token, chunks));

            if (_library.ContainsKey(token))
            {
                _log.Info("returned detail for " + token);
                return Negotiate
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithModel(new JFileDetail(_library[token], chunks));
            }
            else
            {
                _log.Info(token + " not found");
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }
        }

        public dynamic GetFileByChunk(ChunkRequest request)
        {
            _log.Info(string.Format("received chunk request for {0} {1} of {2}", request.Token, request.ChunkNumber, request.Chunks));

            if (_library.ContainsKey(request.Token))
            {
                var fileDetail = new JFileDetail(_library[request.Token], request.Chunks);

                return new Response()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = ContentType.File,
                    Contents = stream =>
                    {
                        var fileStream = new FileStream(
                            _library[request.Token],
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read);

                        fileStream.Seek(fileDetail.Chunks[request.ChunkNumber - 1].Offset, SeekOrigin.Begin);

                        var buffer = new byte[32768];
                        int read;
                        long totalRead = 0;

                        while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0 && totalRead < fileDetail.ChunkSize)
                        {
                            stream.Write(buffer, 0, read);
                            totalRead += read;
                        }
                    }
                };
            }
            else
            {
                _log.Info(request.Token + " not found");
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }
        }
        #endregion
    }
}