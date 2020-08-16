using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace retrowebcore.Models
{
    public class BoardDetail
    {
        

        public static readonly List<CardType> CardTypes = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
        public static readonly Dictionary<CardType, string> Label = new Dictionary<CardType, string>()
        {
            { CardType.Lacked,     "Lacked"      },
            { CardType.LongedFor,  "Longed For"  },
            { CardType.Learned,    "Learned"     },
            { CardType.Liked,      "Liked"       },
            { CardType.ActionItem, "Action Item" }
        };

        public Dictionary<CardType, List<Card>> Cards = new Dictionary<CardType, List<Card>>();
        
        public Guid? Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public BoardDetail(Board board) 
        {
            if (board == null)
                throw new Exception("dont give null to BoardDetail constructor");
            Slug = board.Slug;
            Name = board.Name;
            Description = board.Description;
            Cards[CardType.Liked] = board.Cards?.Where(x => x.CardType == CardType.Liked).ToList();
            Cards[CardType.LongedFor] = board.Cards?.Where(x => x.CardType == CardType.LongedFor).ToList();
            Cards[CardType.Learned] = board.Cards?.Where(x => x.CardType == CardType.Learned).ToList();
            Cards[CardType.Lacked] = board.Cards?.Where(x => x.CardType == CardType.Lacked).ToList();
            Cards[CardType.ActionItem] = board.Cards?.Where(x => x.CardType == CardType.ActionItem).ToList();
        }
    }
}
