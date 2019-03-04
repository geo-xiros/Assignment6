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
        public IHttpActionResult Post(AddDocumentView documentView)
        {
            Document document = _db
                .Documents
                .Add(documentView.Title, documentView.Body);

            if (document == null)
            {
                return NotFound();
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
                return  NotFound();
            }

            // TODO: return row content using razor.parse
            return Ok("<div class=\"row\">" +
                "  <div class=\"row col-12 my-3 p-3 bg-white rounded shadow-sm\">" +
                "    <div class=\"col-9\">" +
                $"      <h4>{documentView.Title}</h4>" +
                "    </div>" +
                "    <div class=\"col-1\">" +
                $"      <a class=\"btn btn-success\" href = \"/Home/Complete?Id={documentAssign.DocumentId}&documentAssignId={documentAssign.Id}&roleId={documentView.RoleId}&userId={documentView.UserId}\">Complete</a>" +
                "    </div>" +
                $"    <div class=\"col-1 \" id=\"{documentAssign.Id}\" UserId=\"{ documentView.UserId}\" body=\"{documentView.Body}\" title=\"{ documentView.Title}\">" +
                "      <button class=\"btn btn-info edit-doc ml-2\">View</button>" +
                "    </div>" +
                "  </div>" +
                "</div>");

        }
    }
}
