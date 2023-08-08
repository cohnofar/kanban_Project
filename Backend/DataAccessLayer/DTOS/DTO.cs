using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOS
{
        internal abstract class DTO
        {
            public const string IDTable = "ID";
            protected Mapper _controller;
            protected bool isPersistent = false;
            public int Id { get; set; } = -1;
            protected DTO(Mapper controller)
            {
                _controller = controller;
            }
            public bool IsPersistent
            {
            get { return isPersistent; }
            set { isPersistent = value; }
              }
        public Mapper Controller { get { return _controller; } }

    }
    }

