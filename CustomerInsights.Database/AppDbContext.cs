using CustomerInsights.ApiService.Models;
using CustomerInsights.Base.Models;
using CustomerInsights.Models;
using CustomerInsights.SignalWorker.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Reflection.Emit;

namespace CustomerInsights.Database;

public class AppDbContext : DbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Interaction> Interactions => Set<Interaction>();
    public DbSet<SatisfactionState> SatisfactionStates => Set<SatisfactionState>();
    public DbSet<TextInference> TextInferences => Set<TextInference>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Signal> Signals => Set<Signal>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // -------------------- Account --------------------
        b.Entity<Account>(cfg =>
        {
            cfg.ToTable("accounts");
            cfg.HasKey(a => a.Id);

            cfg.Property(a => a.Id).HasColumnName("id");
            cfg.Property(a => a.Name).HasColumnName("name").HasMaxLength(256).IsRequired();
            cfg.Property(a => a.ExternalId).HasColumnName("external_id").HasMaxLength(64).IsRequired();
            cfg.Property(a => a.ParentAccountId).HasColumnName("parent_account_id");
            cfg.Property(a => a.Industry).HasColumnName("industry").HasMaxLength(128);
            cfg.Property(a => a.Country).HasColumnName("country").HasMaxLength(64);
            cfg.Property(a => a.Classification).HasColumnName("classification").HasConversion<int>();
            cfg.Property(a => a.CreatedAt).HasColumnName("created_at");

            cfg.HasOne(a => a.ParentAccount)
               .WithMany()
               .HasForeignKey(a => a.ParentAccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            cfg.HasOne(a => a.SatisfactionState)
               .WithOne()
               .HasForeignKey<SatisfactionState>(s => s.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

            cfg.HasMany(a => a.Contacts)
               .WithOne(c => c.Account)
               .HasForeignKey(c => c.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

            cfg.HasMany(a => a.Interactions)
               .WithOne(i => i.Account)
               .HasForeignKey(i => i.AccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
        });

        // -------------------- Contact --------------------
        b.Entity<Contact>(cfg =>
        {
            cfg.ToTable("contacts");
            cfg.HasKey(c => c.Id);

            cfg.Property(c => c.Id).HasColumnName("id");
            cfg.Property(c => c.ExternalId).HasColumnName("external_id").HasMaxLength(64);
            cfg.Property(c => c.Firstname).HasColumnName("firstname").HasMaxLength(128);
            cfg.Property(c => c.Lastname).HasColumnName("lastname").HasMaxLength(128);
            cfg.Property(c => c.Email).HasColumnName("email").HasMaxLength(256);
            cfg.Property(c => c.Phone).HasColumnName("phone").HasMaxLength(64);
            cfg.Property(c => c.CreatedAt).HasColumnName("created_at");

            cfg.Property(c => c.AccountId).HasColumnName("account_id");
            cfg.HasIndex(c => c.AccountId);

            cfg.HasOne(c => c.Account)
               .WithMany(a => a.Contacts)
               .HasForeignKey(c => c.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

            // Neu: korrektes Inverse zu Interaction.Contact
            cfg.HasMany(c => c.Interactions)
               .WithOne(i => i.Contact)
               .HasForeignKey(i => i.ContactId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
        });

        // -------------------- Interaction --------------------
        b.Entity<Interaction>(cfg =>
        {
            cfg.ToTable("interactions");
            cfg.HasKey(i => i.Id);

            cfg.Property(i => i.Id).HasColumnName("id");
            cfg.Property(i => i.TenantId).HasColumnName("tenant_id");
            cfg.Property(i => i.Source).HasColumnName("source").HasMaxLength(128);
            cfg.Property(i => i.ExternalId).HasColumnName("external_id").HasMaxLength(256);
            cfg.Property(i => i.Channel)
               .HasColumnName("channel")
               .HasConversion<int>(); // Enum -> int

            cfg.Property(i => i.OccurredAt).HasColumnName("occurred_at");
            cfg.Property(i => i.AnalyzedAt).HasColumnName("analyzed_at");

            cfg.Property(i => i.AccountId).HasColumnName("account_id");
            cfg.Property(i => i.ContactId).HasColumnName("contact_id");
            cfg.Property(i => i.ThreadId).HasColumnName("thread_id");

            cfg.Property(i => i.Subject).HasColumnName("subject").HasMaxLength(512);
            cfg.Property(i => i.Text).HasColumnName("text");
            cfg.Property(i => i.Meta).HasColumnName("meta");

            cfg.HasIndex(i => i.AccountId);
            cfg.HasIndex(i => i.ContactId);

            // Neu: Beziehung Interaction → Account (optional, SET NULL)
            cfg.HasOne(i => i.Account)
               .WithMany(a => a.Interactions)
               .HasForeignKey(i => i.AccountId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

            // Neu: Beziehung Interaction → Contact (optional, SET NULL)
            cfg.HasOne(i => i.Contact)
               .WithMany(c => c.Interactions)
               .HasForeignKey(i => i.ContactId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);

            // 1:1 Interaction -> TextInference (PK=FK auf TextInference.InteractionId)
            cfg.HasOne(i => i.TextInference)
               .WithOne()
               .HasForeignKey<TextInference>(ti => ti.InteractionId)
               .OnDelete(DeleteBehavior.Cascade);
        });

        // -------------------- SatisfactionState --------------------
        b.Entity<SatisfactionState>(cfg =>
        {
            cfg.ToTable("satisfaction_state");
            cfg.HasKey(s => new { s.TenantId, s.AccountId }); // Composite PK

            cfg.Property(s => s.TenantId).HasColumnName("tenant_id");
            cfg.Property(s => s.AccountId).HasColumnName("account_id");
            cfg.Property(s => s.LastUpdatedUtc).HasColumnName("last_updated_utc");
            cfg.Property(s => s.SatisfactionIndex).HasColumnName("satisfaction_index");
            cfg.Property(s => s.DecayedWeightedSum).HasColumnName("decayed_weighted_sum");
            cfg.Property(s => s.DecayedWeightSum).HasColumnName("decayed_weight_sum");
            cfg.Property(s => s.ConfigVersion).HasColumnName("config_version").HasMaxLength(64);
        });

        // -------------------- TextInference (+ Aspects/Emotions) --------------------
        b.Entity<TextInference>(cfg =>
        {
            cfg.ToTable("text_inference");
            cfg.HasKey(ti => ti.InteractionId); // PK = FK zu Interaction

            cfg.Property(ti => ti.InteractionId).HasColumnName("interaction_id");
            cfg.Property(ti => ti.Sentiment).HasColumnName("sentiment").HasMaxLength(16);
            cfg.Property(ti => ti.SentimentScore).HasColumnName("sentiment_score");
            cfg.Property(ti => ti.Urgency).HasColumnName("urgency").HasMaxLength(32);
            cfg.Property(ti => ti.UrgencyScore).HasColumnName("urgency_score");
            cfg.Property(ti => ti.InferredAt).HasColumnName("inferred_at");
            cfg.Property(ti => ti.Extra).HasColumnName("extra"); // JSON

            cfg.OwnsMany(ti => ti.Aspects, oc =>
            {
                oc.ToTable("text_inference_aspects");
                oc.WithOwner().HasForeignKey("text_inference_id");
                oc.Property<Guid>("id");
                oc.HasKey("id");

                oc.Property(a => a.AspectName).HasColumnName("aspect_name").HasMaxLength(128);
                oc.Property(a => a.Score).HasColumnName("score");

                oc.Ignore(a => a.TextInferenceId);
            });

            cfg.OwnsMany(ti => ti.Emotions, oc =>
            {
                oc.ToTable("text_inference_emotions");
                oc.WithOwner().HasForeignKey("text_inference_id");
                oc.Property<Guid>("id");
                oc.HasKey("id");

                oc.Property(e => e.Label).HasColumnName("label").HasMaxLength(64);
                oc.Property(e => e.Score).HasColumnName("score");
            });
        });

        // -------------------- OutboxMessage --------------------
        b.Entity<OutboxMessage>(cfg =>
        {
            cfg.ToTable("outbox_messages");
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.Id).HasColumnName("id");
            cfg.Property(x => x.TenantId).HasColumnName("tenant_id");
            cfg.Property(x => x.TargetId).HasColumnName("target_id");
            cfg.Property(x => x.Type).HasColumnName("type").HasMaxLength(128);
            cfg.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            cfg.Property(x => x.ProcessedUtc).HasColumnName("processed_utc");
            cfg.Property(x => x.RetryCount).HasColumnName("retry_count");
            cfg.Property(x => x.ErrorMessage).HasColumnName("error_message");
        });

        // -------------------- Signal --------------------
        b.Entity<Signal>(cfg =>
        {
            cfg.ToTable("signals");
            cfg.HasKey(s => s.Id);

            cfg.Property(s => s.Id).HasColumnName("id");
            cfg.Property(s => s.TenantId).HasColumnName("tenant_id");
            cfg.Property(s => s.AccountId).HasColumnName("account_id");

            cfg.HasOne(s => s.Account)
               .WithMany(a => a.Signals)
               .HasForeignKey(s => s.AccountId)
               .OnDelete(DeleteBehavior.Cascade);

            cfg.Property(s => s.Type).HasColumnName("type").HasMaxLength(64);
            cfg.Property(s => s.Severity)
               .HasColumnName("severity")
               .HasConversion<string>()
               .HasMaxLength(32);
            cfg.Property(s => s.CreatedUtc).HasColumnName("created_utc");
            cfg.Property(s => s.TtlDays).HasColumnName("ttl_days");
            cfg.Property(s => s.DedupeKey).HasColumnName("dedupe_key").HasMaxLength(128);

            cfg.Property(s => s.AccountSatisfactionIndex).HasColumnName("account_satisfaction_index");
            cfg.Property(s => s.Threshold).HasColumnName("threshold");

            cfg.HasIndex(s => s.DedupeKey).IsUnique();
            cfg.HasIndex(s => new { s.TenantId, s.AccountId });
        });
    }
}