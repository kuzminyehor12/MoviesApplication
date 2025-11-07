CREATE OR REPLACE FUNCTION calculate_jaccard(
    p_movie_id integer,
    p_query_ngrams text[]
)
RETURNS numeric
LANGUAGE plpgsql
AS $$
DECLARE
movie_ngrams_count integer;
intersection_count integer;
union_count integer;
BEGIN

SELECT COUNT(t.*)
INTO intersection_count
FROM "TermIndex" ti
INNER JOIN "Terms" t
ON ti."TermId" = t."Id"
WHERE ti."MovieId" = p_movie_id
  AND t."TermType" IN (0, 1)
  AND t."TermText" = ANY(p_query_ngrams);

SELECT COUNT(ti.*)
INTO movie_ngrams_count
FROM "TermIndex" ti
INNER JOIN "Terms" t
ON ti."TermId" = t."Id"
WHERE ti."MovieId" = p_movie_id AND t."TermType" IN (0, 1);

union_count := array_length(p_query_ngrams, 1) + movie_ngrams_count - intersection_count;

    IF union_count = 0 THEN
        RETURN 0.0;
END IF;

RETURN CAST(intersection_count AS numeric) / union_count;

END;
$$;