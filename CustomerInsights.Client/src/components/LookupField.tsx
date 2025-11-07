import {Text, Avatar, Box, Flex, Heading, HoverCard, Link} from "@radix-ui/themes";

interface Props {
    title: string,
    description: string,
    targetUrl: string,
    iconUrl: string | null,
    iconFallback: string,
}

export default function LookupField({ title, description, targetUrl, iconUrl, iconFallback }: Props) {

    return (
            <HoverCard.Root>
                <HoverCard.Trigger>
                    <Link href={targetUrl}>
                        {title}
                    </Link>
                </HoverCard.Trigger>
                <HoverCard.Content maxWidth="300px">
                    <Flex gap="4">
                        <Avatar src={iconUrl} fallback={iconFallback} />
                        <Box>
                            <Heading size="3" as="h3">
                                {title}
                            </Heading>
                            <Text as="div" size="2">
                                {description}
                            </Text>
                        </Box>
                    </Flex>
                </HoverCard.Content>
            </HoverCard.Root>
    )
}