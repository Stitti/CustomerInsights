using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Xml;

namespace CustomerInsights.DataverseWorker
{
    public static class CrmClassExtensions
    {
        static public List<Entity> GetEntitiesByCondition(this IOrganizationService service, string entitynName, string[] conditionFieldNames, object[] conditions, string[] valueFieldNames = null)
        {
            QueryByAttribute currency = new QueryByAttribute();
            currency.ColumnSet = CreateColumnSet(valueFieldNames);
            currency.EntityName = entitynName;
            currency.Attributes.AddRange(conditionFieldNames);
            currency.Values.AddRange(conditions); //standard
            RetrieveMultipleResponse rmr = null;
            try
            {
                rmr = service.Execute(new RetrieveMultipleRequest() { Query = currency }) as RetrieveMultipleResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Entity>();
            }

            if (rmr == null)
                return new List<Entity>();

            EntityCollection ec = rmr.EntityCollection;
            if ((ec == null) || (ec.Entities == null))
                return new List<Entity>();

            return ec.Entities.ToList();
        }

        static public List<Entity> GetEntitiesByCondition(this IOrganizationService service, String entityname, String conditionFieldName, object condition, string[] valueFieldNames = null)
        {
            QueryByAttribute currency = new QueryByAttribute();
            currency.ColumnSet = CreateColumnSet(valueFieldNames);
            currency.EntityName = entityname;
            currency.Attributes.AddRange(new string[] { conditionFieldName });
            currency.Values.AddRange(new object[] { condition }); //standard
            RetrieveMultipleResponse rmr = null;
            try
            {
                rmr = service.Execute(new RetrieveMultipleRequest() { Query = currency }) as RetrieveMultipleResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<Entity>();
            }

            if (rmr == null)
                return new List<Entity>();

            EntityCollection ec = rmr.EntityCollection;
            if ((ec == null) || (ec.Entities == null))
                return new List<Entity>();

            return ec.Entities.ToList();
        }

        public static ColumnSet CreateColumnSet(string[] valueFieldNames)
        {
            ColumnSet columnSet;
            if (valueFieldNames == null)
            {
                columnSet = new ColumnSet(false);
            }
            else
            {
                valueFieldNames = valueFieldNames.Where(x => string.IsNullOrEmpty(x) == false).ToArray();
                columnSet = new ColumnSet(valueFieldNames);
            }
            return columnSet;
        }

        static public Entity GetEntityByCondition(this IOrganizationService service, String entityname, String conditionFieldName, object condition, string[] valueFieldNames = null)
        {
            if (condition == null)
                return null;
           	List<Entity> list = service.GetEntitiesByCondition(entityname, conditionFieldName, condition, valueFieldNames);
			if ((list == null) || (list.Count == 0))
                return null;
            return list.FirstOrDefault();
        }

        static public List<Entity> GetAllEntitiesByEntityName(this IOrganizationService service, String entityName, string[] columnSet = null)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = CreateColumnSet(columnSet);
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = 5000;
            query.PageInfo.ReturnTotalRecordCount = true;
            query.PageInfo.PageNumber = 1;
            EntityCollection entityCollection;

            List<Entity> entities = new List<Entity>();

            int count = 0;
            do
            {
                try
                {
                    entityCollection = service.RetrieveMultiple(query);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    return new List<Entity>();
                }
                entities.AddRange(entityCollection.Entities);

                query.PageInfo.PageNumber = query.PageInfo.PageNumber + 1;
                query.PageInfo.PagingCookie = entityCollection.PagingCookie;

                count += entityCollection.Entities.Count;
            } while (entityCollection.Entities.Count == 5000);

            Console.WriteLine("EntityName: " + entityName + "; TotalCounts: " + count);
            return entities;
        }

        static public void SetLookUpFieldWithEntity(this Entity entity, Entity lookUpEntity, String field)
        {
            if ((lookUpEntity == null) || (entity == null))
                return;

            if (entity.Contains(field) == false)
            {
                entity.Attributes.Add(field, lookUpEntity.ToEntityReference());
                return;
            }

            if (entity.GetAttributeValue<Object>(field) != null)
                return;

            entity[field] = lookUpEntity.ToEntityReference();
        }

        static public void UpdateOptionSetToLookUp(this Entity entity, Entity defaultEntity, String fieldLookup, String fieldOptionSet, String fieldName, SetEntityValuesForOptionSetLookUp setValues, IOrganizationService service, string[] defaultEntityColumnset)
        {
            if ((defaultEntity == null) || (entity == null))
                return;

            if (entity.GetAttributeValue<Object>(fieldLookup) != null)
                return;

            if ((entity.Contains(fieldOptionSet) == true) && (entity.GetAttributeValue<OptionSetValue>(fieldOptionSet) != null))
            {
                String valueOp = service.GetOptionSetLabelByValue(entity.LogicalName, fieldOptionSet, entity.GetAttributeValue<OptionSetValue>(fieldOptionSet).Value);
                Entity e = service.GetEntityByCondition(defaultEntity.LogicalName, fieldName, valueOp, defaultEntityColumnset);
                if (e == null)
                {   //es gibt noch keinen Datensatz, der das Optionset abbildet
                    e = new Entity(defaultEntity.LogicalName);
                    setValues(e, valueOp);
                    service.Create(e);
                }

                if (entity.Contains(fieldLookup) == false)
                    entity.Attributes.Add(fieldLookup, e.ToEntityReference());
                else
                    entity[fieldLookup] = e.ToEntityReference();

                return;
            }

            if (entity.Contains(fieldLookup) == false)
                entity.Attributes.Add(fieldLookup, defaultEntity.ToEntityReference());
            else
                entity[fieldLookup] = defaultEntity.ToEntityReference();
        }

        static public string GetOptionSetLabelByValue(this IOrganizationService service, string entityName, string fieldName, int optionSetValue)
        {
            RetrieveAttributeRequest attReq = new RetrieveAttributeRequest();
            attReq.EntityLogicalName = entityName;
            attReq.LogicalName = fieldName;
            attReq.RetrieveAsIfPublished = false;

            var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
            var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;
            OptionMetadata om = attMetadata.OptionSet.Options.Where(x => x.Value == optionSetValue).FirstOrDefault();
            if (om == null)
                return "";

            LocalizedLabel ll = om.Label.LocalizedLabels.Where(l => l.LanguageCode == 1033).FirstOrDefault();
            if (ll == null)
                return "";
            return ll.Label;
        }

        static public int? GetOptionSetValueByLabel(this IOrganizationService service, string entityName, string fieldName, String optionSetLabel)
        {
            var attReq = new RetrieveAttributeRequest();
            attReq.EntityLogicalName = entityName;
            attReq.LogicalName = fieldName;
            attReq.RetrieveAsIfPublished = false;

            var attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
            var attMetadata = (EnumAttributeMetadata)attResponse.AttributeMetadata;
            OptionMetadata omd = attMetadata.OptionSet.Options.Where(x => x.Label.LocalizedLabels.Where(l => l.LanguageCode == 1031).FirstOrDefault().Label == optionSetLabel).FirstOrDefault();
            if (omd == null)
            {
                omd = attMetadata.OptionSet.Options.Where(x => x.Label.LocalizedLabels.Where(l => l.LanguageCode == 1033).FirstOrDefault().Label == optionSetLabel).FirstOrDefault();
            }

            if (omd == null)
            {
                omd = attMetadata.OptionSet.Options.Where(x => x.Label.LocalizedLabels.FirstOrDefault().Label == optionSetLabel).FirstOrDefault();
            }

            if (omd == null)
            {
                return null;
            }

            return omd.Value;
        }

        static public EnumAttributeMetadata GetOptionSetValues(this IOrganizationService service, String logicalName, string fieldName)
        {
            RetrieveAttributeRequest attReq = new RetrieveAttributeRequest();
            attReq.EntityLogicalName = logicalName;
            attReq.LogicalName = fieldName;
            attReq.RetrieveAsIfPublished = false;

            RetrieveAttributeResponse attResponse = (RetrieveAttributeResponse)service.Execute(attReq);
            return (EnumAttributeMetadata)attResponse.AttributeMetadata;
        }

        static public Boolean CheckFieldValue(this Entity entity, string fieldName, Object fieldValue, IOrganizationService proxy)
        {
            if ((entity == null) || (fieldName == null) || (fieldName.Equals("") == true))
                return false;

            if (entity.Contains(fieldName) == false)
            {
                if (fieldValue == null)
                    return true;
                return false;
            }

            object obj = entity.GetAttributeValue<Object>(fieldName);
            if ((obj == null) && (fieldValue == null))
                return true;

            if (obj == null)
                return false;

            if ((obj is DateTime) && (proxy != null))
            {
                TimeZoneInfo tzi = proxy.GetTimeZone();
                obj = TimeZoneInfo.ConvertTime((DateTime)obj, tzi);
            }

            if (obj.Equals(fieldValue))
                return true;

            return false;
        }

        static public Boolean SetFieldValue<T>(this Entity entity, string fieldName, T fieldValue, IOrganizationService proxy)
        {
            if (CheckFieldValue(entity, fieldName, fieldValue, proxy) == true)
                return false;

            if (entity.Contains(fieldName) == false)
            {
                entity.Attributes.Add(fieldName, fieldValue);
            }
            else
            {
                entity[fieldName] = fieldValue;
            }

            return true;
        }

        /**
         * prueft, ob das Feld den vorgegebenen Wert besitzt
         *
         * \param[in] Entity               Datensatz der geprueft wird
         * \param[in] fieldName            welches Feld wird gerüft
         * \param[in] fieldValue           gegen diesen Wert wird geprueft
         * \param[in] update               fuer updates bleibt auf true, falls es true war
         * \return bool                    Werte wurde nicht gesetzt, da er schon gesetzt wurde
         */
        static public void SetFieldValue<T>(this Entity entity, string fieldName, T fieldValue, ref bool update, IOrganizationService proxy)
        {
            if (CheckFieldValue(entity, fieldName, fieldValue, proxy) == true)
                return;

            if (entity.Contains(fieldName) == false)
            {
                entity.Attributes.Add(fieldName, fieldValue);
            }
            else
            {
                entity[fieldName] = fieldValue;
            }

            update = true;
            return;
        }

        /**
         * prueft, ob das Feld den vorgegebenen Wert besitzt
         *
         * \param[in] Entity               Datensatz der geprueft wird
         * \param[in] fieldName            welches Feld wird gerüft
         * \param[in] fieldValue           gegen diesen Wert wird geprueft
         * \param[in] update               fuer updates bleibt auf true, falls es true war
         * \return bool                    Werte wurde nicht gesetzt, da er schon gesetzt wurde
         */
        static public void SetFieldValue<T>(this Entity entity, ref Entity updateEntity, string fieldName, T fieldValue, IOrganizationService proxy = null)
        {
            if (CheckFieldValue(entity, fieldName, fieldValue, proxy) == true)
                return;

            if (updateEntity.Contains(fieldName) == false)
            {
                updateEntity.Attributes.Add(fieldName, fieldValue);
            }
            else
            {
                updateEntity[fieldName] = fieldValue;
            }
        }

        /**
          * prueft, ob das Feld den vorgegebenen Wert besitzt
          *
          * \param[in] Entity               Datensatz der geprueft wird
          * \param[in] fieldName            welches Feld wird gerüft
          * \param[in] fieldValue           gegen diesen Wert wird geprueft
          * \return Object                  Werte wurde nicht gesetzt, da er schon gesetzt wurde
          */


        /**
         * prueft, ob das Feld den vorgegebenen Wert besitzt
         *
         * \param[in] Entity               Datensatz der geprueft wird
         * \param[in] fieldName            welches Feld wird gerüft
         * \param[in] fieldValue           gegen diesen Wert wird geprueft
         * \return Object                  Werte wurde nicht gesetzt, da er schon gesetzt wurde
         */
        static public DateTime GetFieldValue(this Entity entity, string fieldName, IOrganizationService proxy)
        {
            if (entity.Contains(fieldName) == false)
            {
                return default;
            }

            object obj = entity[fieldName];
            if (obj == null)
                return default;

            if ((obj is DateTime) && (proxy != null))
            {
                TimeZoneInfo tzi = proxy.GetTimeZone();
                if (tzi == null)
                {
                    return (DateTime) obj;
                }
                return TimeZoneInfo.ConvertTime((DateTime)obj, tzi);
            }

            if (obj is DateTime)
                return (DateTime)obj;

            return default;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        static public DateTime GetDateTimeNow(this IOrganizationService proxy)
        {
            if (proxy == null)
            {
                return DateTime.UtcNow;
            }

            TimeZoneInfo tzi = proxy.GetTimeZone();
            if (tzi == null)
            {
                return DateTime.UtcNow;
            }
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, tzi);
        }

        /// <summary>
        /// alle verknuepften Datensaetze aus einer N zu M Bezeihung holen
        /// </summary>
        /// <param Name="Entity">Datensatz von dem aus alle verknuepften Datensaetze erstellt werden sollen</param>
        /// <param Name="mappingTableName">Name der Beziehung, nicht der "Entitaetsname der Beziehung"</param>
        /// <param Name="service">IOrganizationService</param>
        /// <returns></returns>
        static public List<Entity> GetAllEntitiesByRelationNM(this Entity entity, String mappingTableName, IOrganizationService service, string[] columnSet = null, bool onlyActiveEntities = false)
        {
            if (entity == null)
                return new List<Entity>();

            RetrieveEntityRequest rere = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Relationships | EntityFilters.Attributes,
                LogicalName = entity.LogicalName
            };

            RetrieveEntityResponse rero = (RetrieveEntityResponse)service.Execute(rere);
            string primaryKeySource = rero.EntityMetadata.PrimaryIdAttribute;
            if ((rero == null) || (rero.EntityMetadata == null) || (rero.EntityMetadata.ManyToManyRelationships == null) || (rero.EntityMetadata.Attributes == null))
                return new List<Entity>();

            ManyToManyRelationshipMetadata mtm = rero.EntityMetadata.ManyToManyRelationships.Where(x => x.SchemaName.Equals(mappingTableName) == true).FirstOrDefault();
            if (mtm == null)
                return new List<Entity>();

            //2te Entitaet
            string targetEntity = mtm.Entity1LogicalName.Equals(entity.LogicalName) == true ? mtm.Entity2LogicalName : mtm.Entity1LogicalName;
            List<AttributeMetadata> primaryKeyAttributList = service.GetAllAttributeMetadataByEntity(targetEntity);
            if (primaryKeyAttributList == null)
                return new List<Entity>();

            AttributeMetadata primaryKeyUnique = primaryKeyAttributList.Where(x => ((x.IsPrimaryId == true) && (x.RequiredLevel != null) && (AttributeRequiredLevel.SystemRequired.Equals(x.RequiredLevel.Value)))).FirstOrDefault();
            if (primaryKeyUnique == null)
                 return new List<Entity>();
            string targetPrimaryKey = primaryKeyUnique.LogicalName;

            //3te Entitaet (Mapping)
            string mappingEntity = mtm.IntersectEntityName;
            string mappingEntityTargetAttribut = mtm.Entity1LogicalName.Equals(entity.LogicalName) == true ? mtm.Entity2IntersectAttribute : mtm.Entity1IntersectAttribute;
            string mappingEntitySourceAttribut = mtm.Entity1LogicalName.Equals(entity.LogicalName) == false ? mtm.Entity2IntersectAttribute : mtm.Entity1IntersectAttribute;

            QueryExpression queryKeys = new QueryExpression(targetEntity);
            queryKeys.ColumnSet = CreateColumnSet(columnSet);
            if (onlyActiveEntities == true)
            {
                queryKeys.Criteria.AddFilter(GetFilterExpressionStatusActive());
            }

            LinkEntity linkEntity1 = new LinkEntity(targetEntity, mappingEntity, targetPrimaryKey, mappingEntityTargetAttribut, JoinOperator.Inner);
            LinkEntity linkEntity2 = new LinkEntity(mappingEntity, entity.LogicalName, mappingEntitySourceAttribut, primaryKeySource, JoinOperator.Inner);

            linkEntity1.LinkEntities.Add(linkEntity2);
            queryKeys.LinkEntities.Add(linkEntity1);

            linkEntity2.LinkCriteria = new FilterExpression();
            linkEntity2.LinkCriteria.AddCondition(new ConditionExpression(primaryKeySource, ConditionOperator.Equal, entity.Id));

            EntityCollection collKeyRecords = service.RetrieveMultiple(queryKeys);
            if ((collKeyRecords == null) || (collKeyRecords.Entities == null) || (collKeyRecords.Entities.Count == 0))
            {
                return new List<Entity>();
            }
            return collKeyRecords.Entities.ToList();
        }

        static public List<Entity> GetAllEntitiesByRelation1N(this Entity entity, String mappingTableName, IOrganizationService service, string[] columnSet = null, bool onlyActiveEntities = false)
        {
            return entity.ToEntityReference().GetAllEntitiesByRelation1N(mappingTableName, service, columnSet, onlyActiveEntities);
        }

        static public List<Entity> GetAllEntitiesByRelation1N(this EntityReference entity, String mappingTableName, IOrganizationService service, string[] columnSet = null, bool onlyActiveEntities = false)
        {
            RetrieveEntityRequest rere = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Relationships,
                LogicalName = entity.LogicalName
            };

            RetrieveEntityResponse rero = (RetrieveEntityResponse)service.Execute(rere);
            String primaryKeySource = rero.EntityMetadata.PrimaryIdAttribute;
            if (rero.EntityMetadata.OneToManyRelationships == null)
                return new List<Entity>();

            OneToManyRelationshipMetadata otm = rero.EntityMetadata.OneToManyRelationships.Where(x => x.ReferencedEntityNavigationPropertyName.Equals(mappingTableName) == true).FirstOrDefault();
            if (otm == null)
            {
                return new List<Entity>();
            }

            QueryExpression queryKeys = new QueryExpression((entity.LogicalName.Equals(otm.ReferencedEntity) == true) ? otm.ReferencingEntity : otm.ReferencedEntity);
            queryKeys.ColumnSet = CreateColumnSet(columnSet);

            FilterExpression filterExpression = new FilterExpression();
            if (onlyActiveEntities == true)
            {
                ConditionExpression conditionStateCode = new ConditionExpression();
                conditionStateCode.AttributeName = "statecode";  //ueberall gleich
                conditionStateCode.Operator = ConditionOperator.Equal;
                conditionStateCode.Values.Add(0);
                filterExpression.Conditions.Add(conditionStateCode);
            }
            ConditionExpression conditionReference = new ConditionExpression();
            conditionReference.AttributeName = otm.ReferencingAttribute;
            conditionReference.Operator = ConditionOperator.Equal;
            conditionReference.Values.Add(entity.Id);
            filterExpression.Conditions.Add(conditionReference);

            queryKeys.Criteria.AddFilter(filterExpression);

            Collection<Entity> entityList = service.RetrieveMultiple(queryKeys).Entities;

            if ((entityList == null) || (entityList.Count == 0))
            {
                return new List<Entity>();
            }
            return entityList.ToList();

        }

        static public List<Entity> GetAllEntitiesByFieldRelation1N(this EntityReference entity, String mappingFieldName, IOrganizationService service, string[] columnSet = null)
        {
            ColumnSet cs = CreateColumnSet(columnSet);
            RetrieveEntityRequest rere = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Relationships,
                LogicalName = entity.LogicalName
            };

            RetrieveEntityResponse rero = (RetrieveEntityResponse)service.Execute(rere);
            String primaryKeySource = rero.EntityMetadata.PrimaryIdAttribute;
            if (rero.EntityMetadata.OneToManyRelationships == null)
                return new List<Entity>();

            OneToManyRelationshipMetadata otm = rero.EntityMetadata.OneToManyRelationships.Where(x => x.ReferencingAttribute.Equals(mappingFieldName) == true || x.ReferencedAttribute.Equals(mappingFieldName) == true).ToList().FirstOrDefault();
            if (otm == null)
            {
                 return new List<Entity>();
            }
            QueryExpression queryKeys = new QueryExpression(otm.ReferencedEntity);
            queryKeys.ColumnSet = cs;

            QueryByAttribute query = new QueryByAttribute();
            query.Attributes.AddRange(new string[] { otm.ReferencingAttribute });
            query.ColumnSet = cs;
            query.EntityName = otm.ReferencingEntity;
            query.Values.AddRange(new object[] { entity.Id });

            Collection<Entity> entityList = ((RetrieveMultipleResponse)service.Execute(new RetrieveMultipleRequest() { Query = query })).EntityCollection.Entities;

            if ((entityList == null) || (entityList.Count == 0))
            {
                return new List<Entity>();
            }
            return entityList.ToList();

        }


        static private FilterExpression GetFilterExpressionStatusActive()
        {
            ConditionExpression condition = new ConditionExpression();
            condition.AttributeName = "statecode";  //ueberall gleich
            condition.Operator = ConditionOperator.Equal;
            condition.Values.Add(0);
            FilterExpression filterExpression = new FilterExpression();
            filterExpression.Conditions.Add(condition);

            return filterExpression;
        }

        static public RelationshipMetadataBase GetRelationShipMetaDatas(this IOrganizationService service, string logicalName, String mappingTableName)
        {

            RetrieveEntityRequest rere = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Relationships,
                LogicalName = logicalName
            };

            RetrieveEntityResponse rero = (RetrieveEntityResponse)service.Execute(rere);
            String primaryKeySource = rero.EntityMetadata.PrimaryIdAttribute;
            if (rero.EntityMetadata.OneToManyRelationships != null)
            {
                OneToManyRelationshipMetadata otm = rero.EntityMetadata.OneToManyRelationships.Where(x => x.ReferencedEntityNavigationPropertyName.Equals(mappingTableName) == true).FirstOrDefault();
                if (otm != null)
                    return otm;
            }

            if (rero.EntityMetadata.ManyToOneRelationships != null)
            {
                OneToManyRelationshipMetadata otm = rero.EntityMetadata.ManyToOneRelationships.Where(x => x.ReferencedEntityNavigationPropertyName.Equals(mappingTableName) == true).FirstOrDefault();
                if (otm != null)
                    return otm;
            }

            if (rero.EntityMetadata.ManyToManyRelationships != null)
            {
                ManyToManyRelationshipMetadata otm = rero.EntityMetadata.ManyToManyRelationships.Where(x => x.IntersectEntityName.Equals(mappingTableName) == true).FirstOrDefault();
                if (otm != null)
                    return otm;
            }

            return null;
        }

        /**
       * gibt den EntityTypeCode zurueck, wird bei der Linkberechnung gebraucht
       *
       * \param[in]  EntityName           Entitaetsname
       * \param[in]  service              IOrganizationService
       *
       * \return int                      EntityTypeCode
       */
        public static int GetEntityTypeCode(string EntityName, IOrganizationService service)
        {
            RetrieveEntityRequest request = new RetrieveEntityRequest();
            request.LogicalName = EntityName;
            RetrieveEntityResponse response = service.Execute(request) as RetrieveEntityResponse;
            if (response == null)
                return -1;

            return response.EntityMetadata.ObjectTypeCode.Value;
        }

        /**
        *  Die Datumswerte müssen alle in die richtige Zeitzone gebracht werden, ansonsten gibt es unschoene Effekte.
        *  z.B. TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, m_timeZone);
        *
        * \param[in]  service           IOrganizationService
        *
        * \return TimeZoneInfo          Zeitzone des SystemUsers
        */
        public static TimeZoneInfo GetTimeZone(this IOrganizationService service)
        {
            DataCollection<Entity> currentUserSettings = service.RetrieveMultiple(
                new QueryExpression("usersettings")
                {
                    ColumnSet = new ColumnSet("localeid", "timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                           {
                                new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
                           }
                    }
                }
             ).Entities;

            TimeZoneInfo timeZoneInfo = null;
            if ((currentUserSettings?.Count > 0) && (currentUserSettings[0] != null) && (currentUserSettings[0].ToEntity<Entity>() != null))
            {
                Entity currentUserSetting = currentUserSettings[0].ToEntity<Entity>();
                var qe = new QueryExpression("timezonedefinition");
                qe.ColumnSet = new ColumnSet("standardname");
                qe.Criteria.AddCondition("timezonecode", ConditionOperator.Equal, currentUserSetting.Attributes["timezonecode"]);
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(service.RetrieveMultiple(qe).Entities.First().GetAttributeValue<string>("standardname"));
            }
            return (timeZoneInfo != null) ? timeZoneInfo : TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
        }

        /// <summary>
        /// // Empfaenger hinzufuegen
        /// </summary>
        /// <param Name="eMail">Datensatz EMail</param>
        /// <param Name="eMailAddress">Emailaddresse als string</param>
        static public void AddReceiverEMail(Entity eMail, string eMailAddress)
        {
            if ((eMailAddress == null) || (eMailAddress.Equals("") == false) || (eMail == null))
            {
                return;
            }

            Entity emailToReciepent = new Entity("activityparty");
            emailToReciepent["addressused"] = eMailAddress;
            if (eMail.Contains("to") == true)
            {
                EntityCollection collectionRecipientMail = eMail["to"] as EntityCollection;
                collectionRecipientMail.Entities.Add(emailToReciepent);
            }
            else
            {
                EntityCollection collectionRecipientMail = new EntityCollection();
                collectionRecipientMail.Entities.Add(emailToReciepent);
                eMail.Attributes.Add("to", collectionRecipientMail);
            }
        }

        public static ExecuteMultipleRequest CreateExecuteMultipleRequest()
        {
            return new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                Requests = new OrganizationRequestCollection()
            };
        }

        /// <summary>
        /// ertsellt eine Entitaet mit DuplicateDetection
        /// </summary>
        /// <param Name="service"></param>
        /// <param Name="Entity">zu erstellender Datensatz</param>
        /// <returns>ID des neuen datensatzes oder des erkannten Duplikates</returns>
        public static Guid CreateEntityWithDuplicateDetection(this IOrganizationService service, Entity entity)
        {
            if ((entity == null) || (service == null))
                return Guid.Empty;


            if (entity.LogicalName.Equals("uomschedule") == true) //Einheitsgruppen Duplikaterkennung automatisch nicht möglich
            {
                Console.WriteLine($"Find {entity.LogicalName}");

                Entity einheitsgruppen = service.GetEntityByCondition("uomschedule", "name", entity.Attributes.Where(x=> (x.Key.Equals("name") == true)).FirstOrDefault().Value);
                if(einheitsgruppen != null)
                {
                    return einheitsgruppen.Id;
                }

            }

            if (entity.LogicalName.Equals("uom") == true) //Einheiten in Einheitsgruppe Duplikaterkennung automatisch nicht möglich
            {
                Console.WriteLine($"Find {entity.LogicalName}");

                Entity einheit = service.GetEntityByCondition("uom", "name", entity.Attributes.Where(x => (x.Key.Equals("name") == true)).FirstOrDefault().Value);
                if (einheit != null)
                {
                    return einheit.Id;
                }
            }

            CreateRequest reqCreate = new CreateRequest();
            reqCreate.Target = entity;
            reqCreate.Parameters.Add("SuppressDuplicateDetection", false); // Change to false to activate the duplicate detection.

            CreateResponse createResponse = new CreateResponse();
            try
            {
                createResponse = (CreateResponse)service.Execute(reqCreate);
                return createResponse.id;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.Message == "A record was not created or updated because a duplicate of the current record already exists.")
                {
                    if (ex.Detail.ErrorDetails["DuplicateAttributes"] == null)
                    {
                        return Guid.Empty;
                    }
                    else
                    {
                        string attributes = ex.Detail.ErrorDetails["DuplicateAttributes"].ToString();
                        attributes = attributes.Replace("&", "&amp;"); //escape & for xml
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(attributes);

                        XmlNode node = doc.DocumentElement;
                        string value = node.FirstChild.InnerText;
                        string field = node.FirstChild.LocalName;

                        Entity entityDup = service.GetEntityByCondition(entity.LogicalName.Trim(), field.Trim(), value.Trim());
                        if (entityDup == null)
                        {
                            return Guid.Empty;
                        }
                        else
                        {
                            return entityDup.Id;
                        }
                    }
                }
                else
                {
                    Console.WriteLine(ex.Message.ToString());
                    return Guid.Empty;
                }
            }
        }
    }


    public static class CrmMetaDatas
    {
        /// <summary>
        /// Feld fuer den Anzeigenamen in Lookup Feldern herausfinden
        /// </summary>
        /// <param Name="Entity"></param>
        /// <returns></returns>
        static public String GetDisplayFieldNameByLookUpFields(this IOrganizationService service, string logicalName)
        {
            if ((logicalName == null) || (logicalName.Equals("")))
                return "";

            RetrieveEntityRequest rere = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = logicalName
            };

            RetrieveEntityResponse rero = (RetrieveEntityResponse)service.Execute(rere);
            AttributeMetadata[] primaryKeySource = rero.EntityMetadata.Attributes;
            if ((primaryKeySource == null) || (primaryKeySource.Count() == 0))
                return null;

            AttributeMetadata am = primaryKeySource.Where(x => x.IsPrimaryName == true).FirstOrDefault();
            if (am == null)
                return "";

            return am.LogicalName;
        }

        /// <summary>
        /// gibt mir alle Felder, aber nur die MetaDaten dazu
        /// </summary>
        /// <param Name="Entity">logicalName</param>
        /// <returns>Liste mit den Attributen</returns>
        static public List<AttributeMetadata> GetAllAttributeMetadataByEntity(this IOrganizationService service, String logicalName)
        {
            if (logicalName == null)
                return new List<AttributeMetadata>();

            RetrieveEntityRequest rere = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = logicalName
            };

            RetrieveEntityResponse rero = (RetrieveEntityResponse)service.Execute(rere);
            if ((rero == null) || (rero.EntityMetadata == null) || (rero.EntityMetadata.Attributes == null))
                return new List<AttributeMetadata>();

            return new List<AttributeMetadata>(rero.EntityMetadata.Attributes);
        }

        static public string GetValueAsString(this IOrganizationService service, List<AttributeMetadata> attributeMetadatas, string fieldNames, object value, string format = null)
        {
            if ((attributeMetadatas == null) || (fieldNames == null) || (service == null) || (value == null))
            {
                Console.WriteLine("(attributeMetadatas == null) || (entity == null) || (fieldNames == null) || (fieldNames.Count() == 0) || (service == null)");
                return "((attributeMetadatas == null) || (entity == null) || (fieldNames == null) || (fieldNames.Count() == 0) || (service == null))";
            }

            AttributeMetadata am = attributeMetadatas.Where(x => fieldNames.Equals(x.LogicalName)).FirstOrDefault();
            if (am == null)
            {
                if ((fieldNames.Equals("stringformat") == true) && (format != null) && (format.Equals("") == false))
                {
                    return String.Format(format, "");
                }
                Console.WriteLine($"(am == null): fieldname: {fieldNames}");
                return "(am == null)";
            }

            if (am.AttributeType.HasValue == false)
            {
                Console.WriteLine("(am.AttributeType.HasValue == false)");
                return "(am.AttributeType.HasValue == false)";
            }

            AttributeTypeCode atc = am.AttributeType.Value;
            switch (atc)
            {
                case (AttributeTypeCode.String):
                case (AttributeTypeCode.Memo):
                case (AttributeTypeCode.Integer):
                case (AttributeTypeCode.Double):
                case (AttributeTypeCode.Boolean):
                    {
                        break;
                    }

                case (AttributeTypeCode.Lookup):
                case (AttributeTypeCode.Customer):
                case (AttributeTypeCode.Owner):
                    {
                        EntityReference er = value as EntityReference;
                        if ((er == null) || (er.Id == null))
                            break;

                        value = er.Id.ToString() + ":" + er.LogicalName;
                        break;
                    }

                case (AttributeTypeCode.DateTime):
                    {
                        TimeZoneInfo tzi = CrmClassExtensions.GetTimeZone(service);
                        if (tzi != null)
                        {
                            value = TimeZoneInfo.ConvertTime(((DateTime)value), tzi);
                        }

                        if ((format == null) || (format.Equals("") == true))
                        {
                            return ((DateTime)value).ToString("dd.MM.yyyy HH:mm:ss");
                        }
                        break;
                    }

                case (AttributeTypeCode.Decimal):
                    {
                        if ((format == null) || (format.Equals("") == true))
                        {
                            return String.Format("{0:0.00}", value);
                        }
                        break;
                    }
                case (AttributeTypeCode.Virtual):
                    {
                        String typeName = am.AttributeTypeName.Value;
                        if ("MultiSelectPicklistType".Equals(typeName) == false)
                            break;

                        OptionSetValueCollection activities = value as OptionSetValueCollection;
                        if (activities == null)
                            break;

                        string newValue = "";
                        foreach (OptionSetValue valueStringOne in activities)
                        {
                            string label = service.GetOptionSetLabelByValue(am.EntityLogicalName, fieldNames, valueStringOne.Value);
                            newValue += label + ":";
                        }
                        value = newValue.Trim(':');
                        break;
                    }

                case (AttributeTypeCode.Money):
                    {
                        if ((format == null) || (format.Equals("") == true))
                        {
                            return String.Format("{0:0.00}", ((Money)value).Value);
                        }

                        value = ((Money)value).Value;
                        break;
                    }


                case (AttributeTypeCode.Picklist):
                    {
                        OptionSetValue osv = value as OptionSetValue;
                        if (osv == null)
                            break;

                        value = service.GetOptionSetLabelByValue(am.EntityLogicalName, fieldNames, osv.Value);
                        break;
                    }

                case (AttributeTypeCode.Uniqueidentifier):
                    {
                        value = value.ToString() + ":" + am.EntityLogicalName;
                        break;
                    }
            }

            if ((value != null) && ((format == null) || (format.Equals("") == true)))
                return value.ToString();

            if ((value == null) && ((format == null) || (format.Equals("") == true)))
                return "";

            if (value == null)
                return String.Format(new CultureInfo("de-DE"), format, "");

            return String.Format(new CultureInfo("de-DE"), format, value);
        }

        static public Object GetStringAsValue(this IOrganizationService service, List<AttributeMetadata> attributeMetadatas, string fieldNames, string valueString)
        {
            object value = null;
            if ((attributeMetadatas == null) || (fieldNames == null))
            {
                Console.WriteLine("(attributeMetadatas == null) || (fieldNames == null) || (fieldNames.Count() == 0)");
                return "(attributeMetadatas == null) || (fieldNames == null) || (fieldNames.Count() == 0)";
            }

            AttributeMetadata am = attributeMetadatas.Where(x => fieldNames.Equals(x.LogicalName)).FirstOrDefault();
            if (am == null)
            {
                Console.WriteLine($"(am == null): fieldname: {fieldNames}");
                return "(am == null)";
            }

            if (am.AttributeType.HasValue == false)
            {
                Console.WriteLine("(am.AttributeType.HasValue == false)");
                return "(am.AttributeType.HasValue == false)";
            }

            AttributeTypeCode atc = am.AttributeType.Value;
            switch (atc)
            {
                case (AttributeTypeCode.Customer):
                case (AttributeTypeCode.Owner):
                case (AttributeTypeCode.Lookup):
                    {
                        LookupAttributeMetadata amTmp = am as LookupAttributeMetadata;
                        string[] split = valueString.Split(':');
                        if (split.Length == 0)
                            break;

                        if (Guid.TryParse(split[0], out Guid guid) == false)
                        {
                            if (string.IsNullOrEmpty(split[0]) == true)
                                break;

                            foreach (string entityName in amTmp.Targets)
                            {
                                try
                                {
                                    string field = service.GetDisplayFieldNameByLookUpFields(entityName);
                                    Entity entity = service.GetEntityByCondition(entityName, field, split[0]);
                                    value = (entity == null) ? null : entity.ToEntityReference();
                                    if (value != null)
                                        break;
                                }
                                catch { }
                            }

                            break;
                        }

                        if (split.Length == 2)
                        {
                            value = new EntityReference(split[1], guid);
                            break;
                        }

                        foreach (string entityName in amTmp.Targets)
                        {
                            try
                            {
                                Entity e = service.Retrieve(entityName, guid, new ColumnSet(false));
                                value = e.ToEntityReference();
                                break;
                            }
                            catch { }
                        }

                        break;
                    }

                case (AttributeTypeCode.String):
                case (AttributeTypeCode.Memo):
                    {
                        value = valueString;
                        break;
                    }

                case (AttributeTypeCode.DateTime):
                    {
                        value = DateTime.Parse(valueString);
                        break;
                    }
                case (AttributeTypeCode.Virtual):
                    {
                        String typeName = am.AttributeTypeName.Value;
                        if ("MultiSelectPicklistType".Equals(typeName) == false)
                            break;

                        string[] valueStringArray = valueString.Split(':');
                        OptionSetValueCollection activities = new OptionSetValueCollection();
                        foreach (string valueStringOne in valueStringArray)
                        {
                            int? valueOSV = service.GetOptionSetValueByLabel(am.EntityLogicalName, fieldNames, valueStringOne.Trim());
                            if (valueOSV.HasValue == true)
                            {
                                activities.Add(new OptionSetValue(valueOSV.Value));
                            }
                        }
                        value = activities.Count == 0 ? null : activities;
                        break;
                    }

                case (AttributeTypeCode.Decimal):
                    {
                        Decimal.TryParse(valueString, out decimal dec);
                        value = dec;
                        break;
                    }

                case (AttributeTypeCode.Money):
                    {
                        Decimal.TryParse(valueString, out decimal dec);
                        value = new Money(dec);
                        break;
                    }

                case (AttributeTypeCode.Integer):
                    {
                        value = int.Parse(valueString);
                        break;
                    }

                case (AttributeTypeCode.Double):
                    {
                        Double.TryParse(valueString, out double dou);
                        value = dou;
                        break;
                    }

                case (AttributeTypeCode.Boolean):
                    {
                        string[] stTrue = { "ja", "checked", "true", "wahr", "yes", "1" };
                        if(stTrue.Contains(valueString.ToLower().Trim()) == true)
                        {
                            valueString = "true";
                        }
                        else
                        {
                            valueString = "false";
                        }
                        Boolean.TryParse(valueString, out bool boolean);
                        value = boolean;
                        break;
                    }

                case (AttributeTypeCode.Picklist):
                    {
                        int? valueOSV = service.GetOptionSetValueByLabel(am.EntityLogicalName, fieldNames, valueString);
                        if (valueOSV.HasValue == true)
                        {
                            value = new OptionSetValue(valueOSV.Value);
                        }

                        break;
                    }
            }

            return value;
        }
    }
}