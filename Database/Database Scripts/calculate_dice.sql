-- Focus on giving a FIXED BONUS AMOUNT (e.g., 0.5) to the numerator
CREATE OR REPLACE FUNCTION calculate_dice(
    p_movie_id integer,
    p_query_ngrams text[],
    p_bonus_amount numeric DEFAULT 2 -- Adds 2 extra weight to the intersection
)
RETURNS numeric
LANGUAGE plpgsql
AS $$
DECLARE
intersection_count numeric;
    movie_ngrams_count integer;
    -- ... other variables ...
BEGIN
SELECT
    -- Numerator: Base 1.0 + Bonus (0.5) for word matches
    COALESCE(
            SUM(
                    CASE
                        WHEN t."TermText" = ANY(p_query_ngrams)
                            THEN (1.0 + CASE WHEN t."TermType" = 1 THEN p_bonus_amount ELSE 0.0 END)
                        ELSE 0.0
                    END
            ),
            0.0
    ),
    -- Denominator Part 2: Unweighted count of movie terms (as before)
    COALESCE(COUNT(t."Id"), 0)
INTO
    intersection_count,
    movie_ngrams_count
FROM "TermIndex" ti
INNER JOIN "Terms" t ON ti."TermId" = t."Id"
WHERE ti."MovieId" = p_movie_id
  AND t."TermType" IN (0, 1);

-- Denominator Part 1: Unweighted count of query ngrams
DECLARE
unweighted_query_size numeric := array_length(p_query_ngrams, 1);
final_denominator numeric := unweighted_query_size + movie_ngrams_count;
BEGIN
        IF final_denominator = 0.0 THEN RETURN 0.0; END IF;

-- We accept that the numerator can now slightly exceed the standard Dice cap of 1.0,
-- but this gives the desired boosting effect without drastic drops.
RETURN (2.0 * intersection_count) / final_denominator;
END;
END;
$$;