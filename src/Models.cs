public record RootObject(
    string id,
    int timestamp,
    Dictionary<string, KeyFrame> keyframe
);

public record KeyFrame(
    int id,
    string timing_id,
    string bib,
    string name,
    bool is_private,
    Category category,
    Start_block start_block,
    Athletes[] athletes,
    string status,
    string start,
    string time,
    int rank,
    int crank,
    int trend,
    double lat,
    double lon,
    object loc_time,
    CheckPoint[] checkpoints
);

public record Category(
    int id,
    int parent,
    string key,
    string name,
    string parent_key
);

public record Start_block(
    int id,
    int race,
    string key,
    string start_date
);

public record Athletes(
    int id,
    int team,
    string timing_id,
    string name,
    int birth_year,
    string nationality,
    int order,
    bool is_lead,
    bool is_standby
);

public record CheckPoint(
    int id,
    string key,
    string time,
    string status,
    int rank,
    int crank
);
