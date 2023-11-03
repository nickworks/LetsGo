using System;

[Serializable]
public class ResponsePlayerProfile
{
    [Serializable]
    public class RelatedLinks {
        public string challenge = ""; // POST : challenge this player!
        public string games = "";
        public string ladders = "";
        public string tournaments = "";
        public string groups = "";
        public string icon = "";
    }

    public RelatedLinks related;
    public int id;
    public string username;
    public bool professional;
    public float ranking;
    public string country;
    public string language;
    public string about;
    public bool supporter;
    public bool is_bot;
    public object bot_owner = null; // !!
    public string website;
    public string registration_date; // !!
    public object name = null; // !!
    public bool timeout_provisional;
    public Friend.Rating ratings;
    public bool is_friend;
    public object aga_id = null; // !!
    public string ui_class;
    public string icon;
}
