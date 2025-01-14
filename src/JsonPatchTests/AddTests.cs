﻿using System.IO;
using JsonPatch.Operations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tavis;
using Xunit;

namespace JsonPatchTests
{
    public class AddTests
    {
        [Fact]
        public void Add_an_array_element()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/-");

            patchDocument.AddOperation(new AddMergeOperation() { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal(list[2]["author"].ToString(), "James Brown");
        }
        
        [Fact]
        public void Insert_an_array_element()
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0");

            patchDocument.AddOperation(new AddMergeOperation() { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal((string)sample["books"][0]["author"], "James Brown");

        }

        [Fact]
        public void Append_an_array_element()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/-");

            patchDocument.AddOperation(new AddMergeOperation { Path = pointer, Value = new JObject(new[] { new JProperty("author", "James Brown") }) });

            patchDocument.ApplyTo(sample);

            var list = sample["books"] as JArray;

            Assert.Equal(3, list.Count);
            Assert.Equal((string)sample["books"][2]["author"], "James Brown");
        }

        [Fact]
        public void Add_an_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/title");

            patchDocument.AddOperation(new AddMergeOperation() { Path = pointer, Value = new JValue("Little Red Riding Hood") });

            patchDocument.ApplyTo(sample);


            var result = (string)pointer.Find(sample);
            Assert.Equal("Little Red Riding Hood", result);

        }

        [Fact]
        public void Add_an_non_existing_member_property()  // Why isn't this replace?
        {

            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/ISBN");

            patchDocument.AddOperation(new AddMergeOperation() { Path = pointer, Value = new JValue("213324234343") });

            patchDocument.ApplyTo(sample);


            var result = (string)pointer.Find(sample);
            Assert.Equal("213324234343", result);

        }



        [Fact]
        public void Add_an_non_existing_object_noreplace()
        {
            var sample = PatchTests.GetSample2();

            var patchDocument = new PatchDocument();
            var pointer = new JsonPointer("/books/0/attributes");

            patchDocument.AddOperation(new AddMergeOperation { Path = pointer, Value = JToken.Parse("{ age: 15 }") });
            patchDocument.AddOperation(new AddMergeOperation { Path = pointer, Value = JToken.Parse("{ pages: 200 }") });
            patchDocument.ApplyTo(sample);

            var pointerAge = new JsonPointer("/books/0/attributes/age");
            var pointerPages = new JsonPointer("/books/0/attributes/pages");

            var result = (string)pointerAge.Find(sample);
            Assert.Equal("15", result);

            result = (string)pointerPages.Find(sample);
            Assert.Equal("200", result);

        }

        [Fact]
        public void Write_ShouldApplyHyphenToPatch_WhenAppendingAnArray()
        {
            // Arrange
            var expected = JToken.Parse("[{ op: \"add\", path: \"/books/-\", value: \"Little Red Riding Hood\" }]").ToString(Formatting.Indented);

            var patchDocument = new PatchDocument();
            var sut = new AddMergeOperation { Path = new JsonPointer("/books/0"), Value = new JValue("Little Red Riding Hood") };
            
            // Act
            patchDocument.AddOperation(sut);
            var stream = patchDocument.ToStream();
            var reader = new StreamReader(stream);

            // Assert
            var actual = reader.ReadToEnd();
            Assert.Equal(expected, actual);

            // Teardown
            reader.Dispose();
            stream.Dispose();
        }
    }
}
