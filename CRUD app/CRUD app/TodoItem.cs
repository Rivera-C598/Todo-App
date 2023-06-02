using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRUD_app
{
    public class TodoItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; }

        public bool IsDone { get; set; }
    }
}
