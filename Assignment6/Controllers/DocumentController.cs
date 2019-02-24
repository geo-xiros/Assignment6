using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Assignment6.ViewModels;
using Assignment6.Models;

namespace Assignment6.Controllers
{
    public class DocumentController : ApiController
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        // POST: api/Document
        [HttpPost]
        [Authorize(Roles = "Architect,Analyst,Programmer,Tester")]
        public string Post(AddDocumentView documentView)
        {
            Document document = _db
                .Documents
                .Add(documentView.Title, documentView.Body);

            if (document == null)
            {
                return string.Empty; //return NotFound();
            }

            DocumentAssign documentAssign = _db
                .DocumentAssigns
                .Add(new DocumentAssign()
            {
                DocumentId = document.Id,
                AssignedToRoleId = documentView.RoleId,
                PurchasedByUserId = documentView.UserId,
                Status = "Pending"
            });

            if (documentAssign == null)
            {
                return string.Empty;// NotFound();
            }

            return $"<tr id=\"{documentAssign.Id}\" UserId=\"{ documentView.UserId}\" body=\"{documentView.Body}\" title=\"{ documentView.Title}\">" +
                $"<td>{documentView.Title}</td>" +
                "<td>" +
                $"<a class=\"btn btn-success\" href = \"/Home/Complete?Id={documentAssign.Id}&roleId={documentView.RoleId}&userId={documentView.UserId}\">Complete</a>" +
                "<button class=\"btn btn-info edit-doc ml-2\"> View</button> " +
                "</td></tr>";
            //return Ok(new {
            //    Id = documentAssign.Id,
            //    UserId = documentView.UserId,
            //    RoleId = documentView.RoleId,
            //    Title = documentView.Title,
            //    Body = documentView.Body
            //});
        }


    }
}
