import { Box, Text, Flex, Heading, Button } from "@radix-ui/themes";
import imgUrl from "../assets/not_found_bot.png";

export default function NotFound() {
  return (
    <Box style={{ minHeight: "100vh", background: "radial-gradient(circle at top, var(--accent-3), var(--gray-1))" }}>
      <Flex direction="column" align="center" justify="center" gap="5" style={{ height: "100%", padding: "32px 16px" }}>
        <Box style={{marginTop: "50px"}}>
            <img src={imgUrl} alt="Page not found" style={{ width: "40vh", height: "auto", display: "block", opacity: 0.9,}}/>
        </Box>

        <Flex direction="column" align="center" gap="2" style={{ textAlign: "center", maxWidth: "420px" }}>

          <Heading size="7">Oops, nothing here.</Heading>

          <Text size="3" color="gray">
            The page you&apos;re looking for doesn&apos;t exist or may have been
            moved. Check the URL or return to your dashboard.
          </Text>
        </Flex>

        <Flex gap="3" wrap="wrap" justify="center">
          <Button size="3" onClick={() => window.history.back()}>
            Go back
          </Button>

          <Button
            size="3"
            variant="outline"
          >
              Go to dashboard
            </Button>
          </Flex>
        </Flex>
      </Box>
    );
  }