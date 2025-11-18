using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.Base.Models.Responses;
using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Patching
{
    public static class PatchingExtension
    {
        public static void ApplyPatch(this Contact contact, UpdateContactRequest patch)
        {
            if (patch.Firstname.IsSpecified)
                contact.Firstname = patch.Firstname.Value;

            if (patch.Lastname.IsSpecified)
                contact.Lastname = patch.Lastname.Value;

            if (patch.ExternalId.IsSpecified)
                contact.ExternalId = patch.ExternalId.Value;

            if (patch.Email.IsSpecified)
                contact.Email = patch.Email.Value;

            if (patch.Phone.IsSpecified)
                contact.Phone = patch.Phone.Value;

            if (patch.AccountId.IsSpecified)
                contact.AccountId = patch.AccountId.Value;
        }

        public static void ApplyPatch(this Account account, UpdateAccountRequest patch)
        {
            if (patch.Name.IsSpecified)
                account.Name = patch.Name.Value;

            if (patch.ExternalId.IsSpecified)
                account.ExternalId = patch.ExternalId.Value;

            if (patch.Industry.IsSpecified)
                account.Industry = patch.Industry.Value;

            if (patch.Country.IsSpecified)
                account.Country = patch.Country.Value;

            if (patch.Classification.IsSpecified)
                account.Classification = patch.Classification.Value ?? CustomerClassification.None;
        }

        public static void ApplyPatch(this Interaction interaction, UpdateInteractionRequest patch)
        {
            if (!string.IsNullOrWhiteSpace(patch.Subject))
                interaction.Subject = patch.Subject;

            if (patch.ContactId.IsSpecified)
                interaction.ContactId = patch.ContactId.Value;

            if (patch.AccountId.IsSpecified)
                interaction.AccountId = patch.AccountId.Value;

            if (patch.TextInference == null)
                return;

            if (interaction.TextInference == null)
                return;

            TextInference textInference = interaction.TextInference;

            if (patch.TextInference.Emotions is { } emotionsPatch)
            {
                List<EmotionRating> emotions = textInference.Emotions ??= new List<EmotionRating>();

                if (emotionsPatch.Remove?.Length > 0)
                {
                    foreach (string label in emotionsPatch.Remove)
                    {
                        EmotionRating? existing = emotions.FirstOrDefault(e => e.Label == label);
                        if (existing != null)
                            emotions.Remove(existing);
                    }
                }

                if (emotionsPatch.Add?.Length > 0)
                {
                    HashSet<string> existingLabels = new HashSet<string>(emotions.Select(e => e.Label), StringComparer.OrdinalIgnoreCase);

                    foreach (string label in emotionsPatch.Add)
                    {
                        if (existingLabels.Contains(label))
                            continue;

                        emotions.Add(new EmotionRating
                        {
                            Label = label,
                            Score = 100
                        });

                        existingLabels.Add(label);
                    }
                }
            }

            if (patch.TextInference.Aspects is { } aspectsPatch)
            {
                List<AspectRating> aspects = textInference.Aspects ??= new List<AspectRating>();

                if (aspectsPatch.Remove?.Length > 0)
                {
                    foreach (string label in aspectsPatch.Remove)
                    {
                        AspectRating? existing = aspects.FirstOrDefault(e => e.AspectName == label);
                        if (existing != null)
                            aspects.Remove(existing);
                    }
                }

                if (aspectsPatch.Add?.Length > 0)
                {
                    HashSet<string> existingLabels = new HashSet<string>(aspects.Select(e => e.AspectName), StringComparer.OrdinalIgnoreCase);

                    foreach (string label in aspectsPatch.Add)
                    {
                        if (existingLabels.Contains(label))
                            continue;

                        aspects.Add(new AspectRating
                        {
                            AspectName = label,
                            Score = 100
                        });

                        existingLabels.Add(label);
                    }
                }
            }
        }
    }
}
