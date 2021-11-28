using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Github_API_Tests
{
    public class Tests
    {
        const string GithubAPIUsername = "qaTest";
        const string GithubAPIPassword = "088e7d775b6a4ecaf7c1193f250300ad0acbfa91";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_GithubAPI_GetAllIssuesByRepo()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Accept-Client", "Fourth-Monitor");
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.IsTrue(response.ContentType.StartsWith("application/json"));

            var issues = new JsonDeserializer().Deserialize<List<IssueResponse>>(response);

            Assert.Pass();
        }
        [Test]
        public void Test_GithubAPI_CreateNewIssue()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                title = "some title",
                body = "some body",
                labels = new string[] { "bug", "importance:high", "type:UI" }
            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            Assert.IsTrue(response.ContentType.StartsWith("application/json"));

            var issue = new JsonDeserializer().Deserialize<IssueResponse>(response);

            Assert.IsTrue(issue.id > 0);
            Assert.IsTrue(issue.number > 0);
            Assert.IsTrue(!String.IsNullOrEmpty(issue.title));
            Assert.IsTrue(!String.IsNullOrEmpty(issue.body));
        }
        [Test]
        public void Test_GithubAPI_CreateNewIssue_Unauthorized()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            //client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                title = "some title",
                body = "some body",
                labels = new string[] { "bug", "importance:high", "type:UI" }
            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Test]
        public void Test_GithubAPI_CreateNewIssue_MissingTitle()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                body = "some body",
                labels = new string[] { "bug", "importance:high", "type:UI" }
            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
        [Test]
        public void Test_GithubAPI_CreateNewIssue_MissingBody()
        {
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                labels = new string[] { "bug", "importance:high", "type:UI" }
            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
        [Test]
        public void Test_GithubAPI_DeleteIssueComment()
        {
            //Create new comment for issue #6
            var client = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/6/comments");
            client.Timeout = 3000;
            var request = new RestRequest(Method.POST);
            client.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                body = "comment body"
            });
            var response = client.Execute(request);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newComment = new JsonDeserializer().Deserialize<CommentResponse>(response);

            //Delete newly created comment
            var clientDel = new RestClient("https://api.github.com/repos/testnakov/test-nakov-repo/issues/6/comments" + newComment.id);
            clientDel.Timeout = 3000;
            var delRequest = new RestRequest(Method.DELETE);
            clientDel.Authenticator = new HttpBasicAuthenticator(GithubAPIUsername, GithubAPIPassword);
            var delResponse = clientDel.Execute(delRequest);

            Assert.AreEqual(HttpStatusCode.NoContent, delResponse.StatusCode);
        }
    }
}
