CREATE INDEX ON "MovieEmbeddings" USING HNSW ("Vector" vector_cosine_ops)
    WITH (M = 16, ef_construction = 64);