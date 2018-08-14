using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace TextFileClassifier.Core
{
    public class TextDocument
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        [IsRetrievable(true)]
        public string DocumentId { get; set; }

        [IsSearchable]
        [IsRetrievable(true)]
        public string Content { get; set; }
    }
}