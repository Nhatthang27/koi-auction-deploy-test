using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Dto.Mail
{
    public class MailDto
    {
        public required int UserId { get; set; }
        public required string Subject { get; set; }
        public required string Text { get; set; }
    }
}