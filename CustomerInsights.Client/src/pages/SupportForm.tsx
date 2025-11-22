import * as React from 'react';
import {
    Box,
    Card,
    Flex,
    Grid,
    Text,
    TextField,
    TextArea,
    Select,
    Button,
    Separator,
    Callout, Tooltip,
} from '@radix-ui/themes';
import {CircleAlertIcon, InfoIcon, SendHorizonalIcon} from 'lucide-react';
import { AttachmentUploader } from '../components/AttatchmentUploader';

type Priority = 'low' | 'normal' | 'high' | 'urgent';
type Category = 'bug' | 'billing' | 'account' | 'feature' | 'other';

interface SupportFormValues {
    firstname: string;
    lastname: string;
    email: string;
    phone: string;
    subject: string;
    description: string;
    category: Category;
    subcategory?: string;
    priority: Priority;
    includeTechnicalDetails: boolean;
    attachments: File[];
}

interface SupportFormProps {
    onSubmit?: (values: SupportFormValues) => Promise<void> | void;
    isSubmittingExternal?: boolean;
}

export const SupportForm: React.FC<SupportFormProps> = ({
                                                            onSubmit,
                                                            isSubmittingExternal,
                                                        }) => {

    const phoneRegex = /^\+?[0-9]{1,3}[\s\-\.]?\(?[0-9]{3}\)?[\s\-\.]?[0-9]{3}[\s\-\.]?[0-9]{4,6}$/i;
    const emailRegex = new RegExp(/^\S+@\S+\.\S+$/);

    const [values, setValues] = React.useState<SupportFormValues>({
        firstname: '',
        lastname: '',
        email: '',
        phone: '',
        subject: '',
        description: '',
        category: 'bug',
        subcategory: '',
        priority: 'normal',
        includeTechnicalDetails: true,
        attachments: [],
    });

    const [errors, setErrors] = React.useState<
        Partial<Record<keyof SupportFormValues, string>>
    >({});
    const [isSubmitting, setIsSubmitting] = React.useState(false);
    const [submitError, setSubmitError] = React.useState<string | null>(null);

    const handleChange =
        <K extends keyof SupportFormValues>(key: K) =>
            (value: SupportFormValues[K]) => {
                setValues((prev) => ({ ...prev, [key]: value }));
                setErrors((prev) => ({ ...prev, [key]: undefined }));
            };

    const validate = (): boolean => {
        const newErrors: Partial<Record<keyof SupportFormValues, string>> = {};

        if (!values.firstname.trim())
            newErrors.firstname = 'Bitte gib deinen Vornamen an.';

        if (!values.lastname.trim())
            newErrors.lastname = 'Bitte gib deinen Nachnamen an.';

        if (!values.email.trim()) {
            newErrors.email = 'Bitte gib deine E-Mail-Adresse an.';
        }
        else if (!emailRegex.test(values.email)) {
            newErrors.email = 'Bitte gib eine gültige E-Mail-Adresse an.';
        }

        if (!values.phone.trim()) {
            newErrors.phone = 'Bitte gib deine Telefonnummer an.';
        }
        else if (!phoneRegex.test(values.phone)) {
            newErrors.phone = 'Bitte gib eine gültige Telefonnummer an.';
        }

        if (!values.subject.trim())
            newErrors.subject = 'Bitte gib einen Betreff an.';

        if (!values.description.trim())
            newErrors.description = 'Bitte beschreibe dein Anliegen.';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit: React.FormEventHandler<HTMLFormElement> = async (e) => {
        e.preventDefault();
        setSubmitError(null);

        if (!validate()) return;

        try {
            setIsSubmitting(true);
            if (onSubmit) {
                await onSubmit(values);
            } else {
                console.log('Support request submitted:', values);
            }
        } catch (error) {
            console.error(error);
            setSubmitError(
                'Beim Absenden ist ein Fehler aufgetreten. Bitte versuche es erneut.'
            );
        } finally {
            setIsSubmitting(false);
        }
    };

    const isBusy = isSubmitting || !!isSubmittingExternal;

    return (
        <Box flexGrow="1" p={{ initial: '4', md: '6' }}>
            <Card
                variant="surface"
                size="4"
                style={{
                    maxWidth: 800,
                    margin: '0 auto',
                    borderRadius: 'var(--radius-4)',
                }}
            >
                <Box p={{ initial: '4', md: '5' }}>
                    <form onSubmit={handleSubmit} noValidate>
                        <Flex direction="column" gap="5">
                            <Box>
                                <Text size="5" weight="bold">
                                    Support kontaktieren
                                </Text>
                                <br/>
                                <Text size="2" color="gray" mt="2">
                                    Beschreibe dein Problem – wir melden uns so schnell wie möglich bei dir.
                                </Text>
                            </Box>

                            <Separator size="4" />

                            <Grid columns={{ initial: '1', sm: '2' }} gap="3">
                                <Box>
                                        <Text as="label" size="2" weight="medium">
                                            Firstname
                                        </Text>
                                    <TextField.Root
                                        value={values.firstname}
                                        onChange={(e) => handleChange('firstname')(e.target.value)}
                                        mt="1"
                                        color={errors.firstname ? 'red' : undefined}
                                    />
                                    {errors.firstname && (
                                        <Text size="1" color="red" mt="1">
                                            {errors.firstname}
                                        </Text>
                                    )}
                                </Box>

                                <Box>
                                    <Text as="label" size="2" weight="medium">
                                        Lastname
                                    </Text>
                                    <TextField.Root
                                        value={values.lastname}
                                        onChange={(e) => handleChange('lastname')(e.target.value)}
                                        mt="1"
                                        color={errors.lastname ? 'red' : undefined}
                                    />
                                    {errors.lastname && (
                                        <Text size="1" color="red" mt="1">
                                            {errors.lastname}
                                        </Text>
                                    )}
                                </Box>
                            </Grid>

                            <Grid columns={{ initial: '1', sm: '2' }} gap="3">
                                <Box>
                                    <Text as="label" size="2" weight="medium">
                                        E-Mail
                                    </Text>
                                    <TextField.Root
                                        type="email"
                                        value={values.email}
                                        onChange={(e) => handleChange('email')(e.target.value)}
                                        mt="1"
                                        color={errors.email ? 'red' : undefined}
                                    />
                                    {errors.email && (
                                        <Text size="1" color="red" mt="1">
                                            {errors.email}
                                        </Text>
                                    )}
                                </Box>

                                <Box>
                                    <Text as="label" size="2" weight="medium">
                                        Phone
                                    </Text>
                                    <TextField.Root
                                        type="tel"
                                        value={values.phone}
                                        onChange={(e) => handleChange('phone')(e.target.value)}
                                        mt="1"
                                        color={errors.phone ? 'red' : undefined}
                                    />
                                    {errors.phone && (
                                        <Text size="1" color="red" mt="1">
                                            {errors.phone}
                                        </Text>
                                    )}
                                </Box>
                            </Grid>

                            <Grid columns={{ initial: '1', sm: '2' }} gap="3">
                                <Box>

                                    <Text as="label" size="2" weight="medium">
                                        Betreff
                                        <Tooltip content="Kurze Zusammenfassung deines Anliegens">
                                            <InfoIcon style={{marginLeft: "5px"}} size="12" />
                                        </Tooltip>
                                    </Text>
                                    <TextField.Root
                                        value={values.subject}
                                        onChange={(e) => handleChange('subject')(e.target.value)}
                                        mt="1"
                                        color={errors.subject ? 'red' : undefined}
                                    />
                                    {errors.subject && (
                                        <Text size="1" color="red" mt="1">
                                            {errors.subject}
                                        </Text>
                                    )}
                                </Box>

                                <Box>
                                    <Text as="label" size="2" weight="medium">
                                        Kategorie
                                    </Text>
                                    <br/>
                                    <Select.Root
                                        value={values.category}
                                        onValueChange={(val) => handleChange('category')(val as Category)}
                                    >
                                        <Select.Trigger mt="1" style={{width:'100%'}} />
                                        <Select.Content>
                                            <Select.Item value="bug">Fehler / Bug</Select.Item>
                                            <Select.Item value="billing">Abrechnung & Zahlungen</Select.Item>
                                            <Select.Item value="account">Account & Zugriff</Select.Item>
                                            <Select.Item value="feature">Feature Request</Select.Item>
                                            <Select.Item value="other">Sonstiges</Select.Item>
                                        </Select.Content>
                                    </Select.Root>
                                </Box>
                            </Grid>

                            <Grid columns={{ initial: '1', sm: '2' }} gap="3">
                                <Box>
                                    <Text as="label" size="2" weight="medium">
                                        Bereich / Unterkategorie (optional)
                                        <Tooltip content='z. B. „Dashboard“, „Checkout“, „API“'>
                                            <InfoIcon style={{marginLeft: "5px"}} size="12" />
                                        </Tooltip>
                                    </Text>
                                    <TextField.Root
                                        value={values.subcategory ?? ''}
                                        onChange={(e) => handleChange('subcategory')(e.target.value)}
                                        mt="1"
                                    />
                                </Box>

                                <Box>
                                    <Text as="label" size="2" weight="medium">
                                        Priorität
                                    </Text>
                                    <br/>
                                    <Select.Root
                                        value={values.priority}
                                        onValueChange={(val) => handleChange('priority')(val as Priority)}
                                    >
                                        <Select.Trigger mt="1" style={{width:'100%'}}/>
                                        <Select.Content>
                                            <Select.Item value="low">Niedrig – kein Zeitdruck</Select.Item>
                                            <Select.Item value="normal">Normal</Select.Item>
                                            <Select.Item value="high">
                                                Hoch – beeinflusst meine Arbeit
                                            </Select.Item>
                                            <Select.Item value="urgent">
                                                Kritisch – Systemausfall
                                            </Select.Item>
                                        </Select.Content>
                                    </Select.Root>
                                </Box>
                            </Grid>

                            <Box>
                                <Text as="label" size="2" weight="medium">
                                    Beschreibung
                                </Text>
                                <TextArea
                                    value={values.description}
                                    onChange={(e) => handleChange('description')(e.target.value)}
                                    placeholder="Bitte beschreibe dein Problem so detailliert wie möglich (Schritte, erwartetes Verhalten, tatsächliches Verhalten, Screenshots etc.)."
                                    mt="1"
                                    rows={5}
                                    color={errors.description ? 'red' : undefined}
                                />
                                {errors.description && (
                                    <Text size="1" color="red" mt="1">
                                        {errors.description}
                                    </Text>
                                )}
                            </Box>

                            <Box>
                                <Text as="label" size="2" weight="medium">
                                    Anhänge
                                </Text>
                                <br/>
                                <Text size="1" color="gray">
                                    Screenshots, Logs, CSV, PDFs – max. 10 Dateien
                                </Text>
                                <Box mt="2">
                                    <AttachmentUploader
                                        value={values.attachments}
                                        onChange={(files) => handleChange('attachments')(files)}
                                        maxFiles={10}
                                    />
                                </Box>
                            </Box>

                            {submitError && (
                                <Callout.Root color="red">
                                    <Callout.Icon>
                                        <CircleAlertIcon />
                                    </Callout.Icon>
                                    <Callout.Text>{submitError}</Callout.Text>
                                </Callout.Root>
                            )}

                            {/* Actions */}
                            <Flex justify="end" gap="3">
                                <Button type="submit" disabled={isBusy}>
                                    <SendHorizonalIcon size={16}/>
                                    {isBusy ? 'Wird gesendet…' : 'An Support senden'}
                                </Button>
                            </Flex>
                        </Flex>
                    </form>
                </Box>
            </Card>
        </Box>
    );
};