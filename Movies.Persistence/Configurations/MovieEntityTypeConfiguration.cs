using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Movies.Core.Entities;

namespace Movies.Persistence.Configurations;

public class MovieEntityTypeConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder
            .Property(movie => movie.Id)
            .UseIdentityColumn();
        
        builder
            .HasIndex(movie => movie.Title)
            .IsUnique();
        
        builder
            .Property(movie => movie.CastMembers)
            .HasColumnName("cast")
            .HasColumnType("jsonb")
            .HasConversion(
                cast => JsonSerializer.Serialize(cast, JsonSerializerOptions.Default), 
                json => JsonSerializer.Deserialize<CastMember[]>(json, JsonSerializerOptions.Default) ?? Array.Empty<CastMember>());
        
        builder
            .Property(movie => movie.CrewMembers)
            .HasColumnName("crew")
            .HasColumnType("jsonb")
            .HasConversion(
                crew => JsonSerializer.Serialize(crew, JsonSerializerOptions.Default), 
                json => JsonSerializer.Deserialize<CrewMember[]>(json, JsonSerializerOptions.Default) ?? Array.Empty<CrewMember>());
        
        builder
            .Property(movie => movie.Keywords)
            .HasColumnName("keywords")
            .HasColumnType("jsonb")
            .HasConversion(
                keywords => JsonSerializer.Serialize(keywords, JsonSerializerOptions.Default), 
                json => JsonSerializer.Deserialize<Keyword[]>(json, JsonSerializerOptions.Default) ?? Array.Empty<Keyword>());
        
        builder
            .Property(movie => movie.Genres)
            .HasColumnName("genres")
            .HasColumnType("jsonb")
            .HasConversion(
                genres => JsonSerializer.Serialize(genres, JsonSerializerOptions.Default), 
                json => JsonSerializer.Deserialize<Genre[]>(json, JsonSerializerOptions.Default) ?? Array.Empty<Genre>());;
    }
}