using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZimaHrm.Core.Infrastructure.Extensions;
using Aspose.Pdf.Facades;
using Humanizer;

namespace ZimaHrm.Core.Infrastructure.IO
{
	/// <summary>
	/// Contains methods related to PDF documents.
	/// </summary>
	public static class PdfOps {
		/// <summary>
		/// Concatenates the specified PDF documents and writes the result to the specified output stream.
		/// </summary>
		public static void ConcatPdfs( IEnumerable<Stream> inputStreams, Stream outputStream ) {
			new PdfFileEditor().Concatenate( inputStreams.ToArray(), outputStream );
		}

		/// <summary>
		/// Concatenates the specified PDF documents and creates bookmarks to the beginning of each file, 
		/// specified by the title passed in the Tuple.
		/// </summary>
		/// <param name="outputStream">Stream in which to write</param>
		/// <param name="bookmarkNamesAndPdfStreams">Title to write in the bookmark, PDF MemoryStream</param>
		public static void CreateBookmarkedPdf( IEnumerable<Tuple<string, MemoryStream>> bookmarkNamesAndPdfStreams, Stream outputStream ) {
			var concatPdfsStream = new MemoryStream();
			using( concatPdfsStream ) {
				// Paste all of the PDFs together
				ConcatPdfs( bookmarkNamesAndPdfStreams.Select( p => p.Item2 ), concatPdfsStream );
			}

			// Add bookmarks to PDF
			var bookMarkedPdf = AddBookmarksToPdf( concatPdfsStream.ToArray(), bookmarkNamesAndPdfStreams.Select( t => Tuple.Create( t.Item1, t.Item2.ToArray() ) ) );

			// Have the bookmarks displayed on PDF open
			bookMarkedPdf = SetShowBookmarksPaneOnOpen( bookMarkedPdf );

			// Save the PDf to the output stream
			using( var bookMarksDisplayedPdf = new MemoryStream( bookMarkedPdf ) )
				bookMarksDisplayedPdf.CopyTo( outputStream );
		}

		/// <summary>
		/// Adds a bookmark to the first page of each of the given PDFs in the Tuple's byte array, using the string in that Tuple for the
		/// title of the bookmark.
		/// </summary>
		/// <param name="pdf">PDF byte array</param>
		/// <param name="titleAndPdfs">Tuples of &lt;Bookmark title, PDF byte array&gt;</param>
		/// <returns>PDF byte array</returns>
		private static byte[] AddBookmarksToPdf( byte[] pdf, IEnumerable<Tuple<string, byte[]>> titleAndPdfs ) {
			using( var tmpPdf = new MemoryStream( pdf ) ) {
				var bookmarkEditor = new PdfBookmarkEditor();
				bookmarkEditor.BindPdf( tmpPdf );
				var count = 1;
				foreach( var titleAndPdf in titleAndPdfs ) {
					bookmarkEditor.CreateBookmarkOfPage( titleAndPdf.Item1, count );
					count += new PdfFileInfo( new MemoryStream( titleAndPdf.Item2 ) ).NumberOfPages;
				}

				using( var addBookmarksStream = new MemoryStream() ) {
					bookmarkEditor.Save( addBookmarksStream );
					return addBookmarksStream.ToArray();
				}
			}
		}

		/// <summary>
		/// Sets the PDF to be showing the Bookmarks pane (document outline) on document open.
		/// </summary>
		/// <param name="pdf">PDF byte array</param>
		/// <returns>PDF byte array</returns>
		private static byte[] SetShowBookmarksPaneOnOpen( byte[] pdf ) {
			using( var tmpPdf = new MemoryStream( pdf ) ) {
				var pce = new PdfContentEditor();
				pce.BindPdf( tmpPdf );
				pce.ChangeViewerPreference( ViewerPreference.PageModeUseOutlines );
				using( var saveStream = new MemoryStream() ) {
					pce.Save( saveStream );
					return saveStream.ToArray();
				}
			}
		}

	
		private static void ResetFileStream( params FileStream[] fs ) {
			foreach( var f in fs )
				f.Reset();
		}
	}
}
