import { useCallback, useRef, useState } from 'react';
import './Toast.css';

export function useToast() {
  const [message, setMessage] = useState<string | null>(null);
  const timerRef = useRef<number | undefined>(undefined);

  const showToast = useCallback((text: string) => {
    setMessage(text);
    window.clearTimeout(timerRef.current);
    timerRef.current = window.setTimeout(() => setMessage(null), 1600);
  }, []);

  const toastNode = <div className={`toast${message ? ' is-on' : ''}`}>{message}</div>;

  return { showToast, toastNode };
}
